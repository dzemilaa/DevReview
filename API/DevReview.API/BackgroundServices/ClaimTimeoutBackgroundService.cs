using DevReview.Application.Interfaces;
using DevReview.Domain.Entities;
using DevReview.Domain.Enums;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DevReview.API.BackgroundServices
{
    public class ClaimTimeoutBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<ClaimTimeoutBackgroundService> _logger;

        public ClaimTimeoutBackgroundService(
            IServiceScopeFactory scopeFactory,
            ILogger<ClaimTimeoutBackgroundService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                    var now = DateTime.UtcNow;

                    var expired = await unitOfWork.ReviewRequests.FindAsync(
                        r => (r.Status == ReviewStatus.Claimed || r.Status == ReviewStatus.InReview)
                             && r.ClaimTimeout.HasValue
                             && r.ClaimTimeout < now,
                        stoppingToken);

                    foreach (var request in expired)
                    {
                        request.MentorId = null;
                        request.ClaimedAt = null;
                        request.ClaimTimeout = null;
                        request.Status = ReviewStatus.Open;
                        unitOfWork.ReviewRequests.Update(request);
                        await unitOfWork.Notifications.AddAsync(new Notification
                        {
                            UserId = request.OwnerId,
                            Message = $"The mentor did not complete review '{request.Title}' within 24 hours. It is now open again.",
                            Type = NotificationType.RequestAbandoned
                        }, stoppingToken);
                        _logger.LogInformation(
                            "Released expired claim for review request {ReviewRequestId}",
                            request.Id);
                    }

                    if (expired.Count > 0)
                    {
                        await unitOfWork.SaveChangesAsync(stoppingToken);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Claim timeout job failed");
                }

                await Task.Delay(TimeSpan.FromMinutes(15), stoppingToken);
            }
        }
    }
}
