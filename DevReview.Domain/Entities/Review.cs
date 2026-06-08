using System;

namespace DevReview.Domain.Entities
{
    public class Review
    {
        public Guid Id { get; set; }
        public string Summary { get; set; } = string.Empty;
        public string PriorityFixes { get; set; } = string.Empty;
        public int QualityScore { get; set; }
        public int? AuthorRatingOfMentor { get; set; }
        public int? MentorRatingOfAuthor { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Guid ReviewerId { get; set; }
        public User? Reviewer { get; set; }

        public Guid ReviewRequestId { get; set; }
        public ReviewRequest? ReviewRequest { get; set; }
    }
}
