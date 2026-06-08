using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using DevReview.Application.DTOs;
using DevReview.Application.Interfaces;
using MediatR;

namespace DevReview.Application.Notifications
{
    public class GetNotificationsQueryHandler : IRequestHandler<GetNotificationsQuery, IReadOnlyList<NotificationDto>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;

        public GetNotificationsQueryHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
            _mapper = mapper;
        }

        public async Task<IReadOnlyList<NotificationDto>> Handle(GetNotificationsQuery request, CancellationToken cancellationToken)
        {
            var notifications = await _unitOfWork.Notifications.FindAsync(x => x.UserId == _currentUserService.UserId, cancellationToken);
            if (request.OnlyUnread)
            {
                notifications = notifications.Where(x => !x.IsRead).ToList();
            }

            return notifications.OrderByDescending(x => x.CreatedAt)
                .Select(_mapper.Map<NotificationDto>)
                .ToList();
        }
    }

    public class MarkNotificationReadCommandHandler : IRequestHandler<MarkNotificationReadCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public MarkNotificationReadCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<Unit> Handle(MarkNotificationReadCommand request, CancellationToken cancellationToken)
        {
            var notification = await _unitOfWork.Notifications.GetByIdAsync(request.NotificationId, cancellationToken);
            if (notification == null || notification.UserId != _currentUserService.UserId)
            {
                throw new ApplicationException("Notification not found or access denied.");
            }

            notification.IsRead = true;
            _unitOfWork.Notifications.Update(notification);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }
    }

    public class MarkAllNotificationsReadCommandHandler : IRequestHandler<MarkAllNotificationsReadCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public MarkAllNotificationsReadCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<Unit> Handle(MarkAllNotificationsReadCommand request, CancellationToken cancellationToken)
        {
            var notifications = await _unitOfWork.Notifications.FindAsync(x => x.UserId == _currentUserService.UserId && !x.IsRead, cancellationToken);
            foreach (var notification in notifications)
            {
                notification.IsRead = true;
                _unitOfWork.Notifications.Update(notification);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }
    }
}
