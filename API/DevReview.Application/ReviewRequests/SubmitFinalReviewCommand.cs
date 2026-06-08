using System;
using DevReview.Application.DTOs;
using MediatR;

namespace DevReview.Application.ReviewRequests
{
    public class SubmitFinalReviewCommand : IRequest<ReviewRequestDto>
    {
        public Guid ReviewRequestId { get; set; }
        public Guid MentorId { get; set; }
        public string Summary { get; set; } = string.Empty;
        public string PriorityFixes { get; set; } = string.Empty;
        public int QualityScore { get; set; }
    }
}
