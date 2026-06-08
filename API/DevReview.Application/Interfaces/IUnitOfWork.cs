using System.Threading;
using System.Threading.Tasks;
using DevReview.Domain.Entities;

namespace DevReview.Application.Interfaces
{
    public interface IUnitOfWork
    {
        IGenericRepository<User> Users { get; }
        IGenericRepository<ReviewRequest> ReviewRequests { get; }
        IGenericRepository<CodeFile> CodeFiles { get; }
        IGenericRepository<Comment> Comments { get; }
        IGenericRepository<Review> Reviews { get; }
        IGenericRepository<OfficeHour> OfficeHours { get; }
        IGenericRepository<Tag> Tags { get; }
        IGenericRepository<MentorReputation> MentorReputations { get; }
        IGenericRepository<Notification> Notifications { get; }
        IGenericRepository<UserLanguage> UserLanguages { get; }
        IGenericRepository<UserRefreshToken> UserRefreshTokens { get; }
        IGenericRepository<UserFollow> UserFollows { get; }

        Task<ReviewRequest?> GetReviewRequestWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
