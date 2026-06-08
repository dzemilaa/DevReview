using System;
using DevReview.Domain.Enums;

namespace DevReview.Domain.Entities
{
    public class Notification
    {
        public Guid Id { get; set; }
        public string Message { get; set; } = string.Empty;
        public NotificationType Type { get; set; }
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Guid UserId { get; set; }
        public User? User { get; set; }
    }
}
