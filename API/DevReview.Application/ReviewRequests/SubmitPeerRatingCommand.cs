using MediatR;

namespace DevReview.Application.ReviewRequests
{
    public class SubmitPeerRatingCommand : IRequest<Unit>
    {
        public Guid ReviewRequestId { get; set; }
        public int Rating { get; set; }
    }
}
