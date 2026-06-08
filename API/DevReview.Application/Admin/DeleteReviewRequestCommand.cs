using System;
using MediatR;

namespace DevReview.Application.Admin
{
    public class DeleteReviewRequestCommand : IRequest<Unit>
    {
        public Guid ReviewRequestId { get; set; }
    }

    public class DeleteCommentCommand : IRequest<Unit>
    {
        public Guid CommentId { get; set; }
    }
}
