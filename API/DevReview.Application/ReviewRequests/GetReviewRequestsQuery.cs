using System;
using System.Collections.Generic;
using DevReview.Application.DTOs;
using DevReview.Domain.Enums;
using MediatR;

namespace DevReview.Application.ReviewRequests
{
    public class GetReviewRequestsQuery : IRequest<IReadOnlyList<ReviewRequestDto>>
    {
        public Guid? OwnerId { get; set; }
        public Guid? MentorId { get; set; }
        public ReviewStatus? Status { get; set; }
    }
}
