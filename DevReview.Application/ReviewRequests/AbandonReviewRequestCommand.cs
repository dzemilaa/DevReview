using System;
using DevReview.Application.DTOs;
using MediatR;

namespace DevReview.Application.ReviewRequests
{
    public class AbandonReviewRequestCommand : IRequest<ReviewRequestDto>
    {
        public Guid ReviewRequestId { get; set; }
        public Guid MentorId { get; set; }
        public string Reason { get; set; } = string.Empty;
    }
}
