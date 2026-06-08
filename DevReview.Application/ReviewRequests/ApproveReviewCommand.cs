using System;
using MediatR;

namespace DevReview.Application.ReviewRequests
{
    public class ApproveReviewCommand : IRequest<Unit>
    {
        public Guid ReviewRequestId { get; set; }
        public Guid AuthorId { get; set; }
        public bool Approved { get; set; }
    }
}
