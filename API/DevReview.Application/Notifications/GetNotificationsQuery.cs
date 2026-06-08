using System.Collections.Generic;
using DevReview.Application.DTOs;
using MediatR;

namespace DevReview.Application.Notifications
{
    public class GetNotificationsQuery : IRequest<IReadOnlyList<NotificationDto>>
    {
        public bool OnlyUnread { get; set; }
    }
}
