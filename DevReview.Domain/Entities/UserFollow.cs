using System;

namespace DevReview.Domain.Entities
{
    public class UserFollow
    {
        public Guid FollowerId { get; set; }
        public User? Follower { get; set; }

        public Guid MentorId { get; set; }
        public User? Mentor { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
