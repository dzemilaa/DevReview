using DevReview.Application.DTOs;
using MediatR;

namespace DevReview.Application.ReviewRequests
{
    public class ApplySuggestionCommand : IRequest<CodeFileDto>
    {
        public Guid ReviewRequestId { get; set; }
        public Guid CommentId { get; set; }
        public Guid AuthorId { get; set; }
    }
}
