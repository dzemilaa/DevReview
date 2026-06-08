using System;
using DevReview.Domain.Enums;

namespace DevReview.Application.DTOs
{
    public class ReviewRequestDto
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
        public DateTime CreatedAt { get; set; }
        public Guid OwnerId { get; set; }
        public Guid? MentorId { get; set; }
    }
}
