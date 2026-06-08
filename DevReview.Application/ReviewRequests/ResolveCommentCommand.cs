using DevReview.Application.DTOs;
using MediatR;

namespace DevReview.Application.ReviewRequests
{
    public class ResolveCommentCommand : IRequest<CommentDto>
    {
        public Guid ReviewRequestId { get; set; }
        public Guid CommentId { get; set; }
        public Guid MentorId { get; set; }
    }
}
