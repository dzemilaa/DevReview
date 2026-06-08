using System;

namespace DevReview.Application.DTOs
{
    public class FinalReviewSummaryDto
    {
        public string Summary { get; set; } = string.Empty;
        public string PriorityFixes { get; set; } = string.Empty;
        public int QualityScore { get; set; }
        public DateTime CreatedAt { get; set; }
        public int? AuthorRatingOfMentor { get; set; }
        public int? MentorRatingOfAuthor { get; set; }
    }
}
