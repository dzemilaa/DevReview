using System;
using System.Collections.Generic;
using DevReview.Domain.Enums;

namespace DevReview.Application.DTOs
{
    public class ReviewRequestDetailDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ProgrammingLanguage { get; set; } = string.Empty;
        public string Framework { get; set; } = string.Empty;
        public DifficultyLevel Difficulty { get; set; }
        public bool IsPublic { get; set; }
        public bool IsPaid { get; set; }
        public decimal Price { get; set; }
        public IList<string> Tags { get; set; } = new List<string>();
        public ReviewStatus Status { get; set; }
        public Guid OwnerId { get; set; }
        public Guid? MentorId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ClaimedAt { get; set; }
        public DateTime? ClaimTimeout { get; set; }
        public DateTime? CompletedAt { get; set; }
        public IList<CodeFileDto> CodeFiles { get; set; } = new List<CodeFileDto>();
        public IList<CommentDto> Comments { get; set; } = new List<CommentDto>();
        public FinalReviewSummaryDto? FinalReview { get; set; }
    }
}
