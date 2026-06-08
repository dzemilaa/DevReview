using System.Collections.Generic;
using DevReview.Application.DTOs;
using DevReview.Domain.Enums;
using MediatR;

namespace DevReview.Application.ReviewRequests
{
    public class CreateReviewRequestCommand : IRequest<ReviewRequestDto>
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ProgrammingLanguage { get; set; } = string.Empty;
        public string Framework { get; set; } = string.Empty;
        public DifficultyLevel Difficulty { get; set; } = DifficultyLevel.Junior;
        public bool IsPublic { get; set; }
        public bool IsPaid { get; set; }
        public decimal Price { get; set; }
        public Guid OwnerId { get; set; }
        public IList<string> Tags { get; set; } = new List<string>();
        public IList<CodeFileDto> CodeFiles { get; set; } = new List<CodeFileDto>();
    }
}
