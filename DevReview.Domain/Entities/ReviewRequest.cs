using System;
using System.Collections.Generic;
using DevReview.Domain.Enums;

namespace DevReview.Domain.Entities
{
    public class ReviewRequest
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ProgrammingLanguage { get; set; } = string.Empty;
        public string Framework { get; set; } = string.Empty;
        public DifficultyLevel Difficulty { get; set; } = DifficultyLevel.Junior;
        public bool IsPublic { get; set; }
        public bool IsPaid { get; set; }
        public decimal Price { get; set; }
        public string Tags { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public ReviewStatus Status { get; set; } = ReviewStatus.Open;
        public Guid OwnerId { get; set; }
        public User? Owner { get; set; }
        public Guid? MentorId { get; set; }
        public User? Mentor { get; set; }
        public DateTime? ClaimedAt { get; set; }
        public DateTime? ClaimTimeout { get; set; }
        public DateTime? CompletedAt { get; set; }

        public ICollection<CodeFile> CodeFiles { get; set; } = new List<CodeFile>();
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
}
