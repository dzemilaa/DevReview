using DevReview.Application.DTOs;
using MediatR;

namespace DevReview.Application.ReviewRequests
{
    public class GetPublicReviewRequestsQuery : IRequest<IReadOnlyList<ReviewRequestDto>>
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}
