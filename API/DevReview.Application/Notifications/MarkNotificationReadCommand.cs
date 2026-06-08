using System;
using MediatR;

namespace DevReview.Application.Notifications
{
    public class MarkNotificationReadCommand : IRequest<Unit>
    {
        public Guid NotificationId { get; set; }
    }
}
