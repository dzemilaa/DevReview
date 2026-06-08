using System;
using DevReview.Application.DTOs;
using MediatR;

namespace DevReview.Application.ReviewRequests
{
    public class GetReviewRequestByIdQuery : IRequest<ReviewRequestDetailDto>
    {
        public Guid ReviewRequestId { get; set; }
        public bool RequirePublic { get; set; }
    }
}
