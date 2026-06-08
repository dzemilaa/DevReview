using System;
using DevReview.Application.DTOs;
using DevReview.Domain.Enums;
using MediatR;

namespace DevReview.Application.ReviewRequests
{
    public class AddCommentCommand : IRequest<CommentDto>
    {
        public Guid ReviewRequestId { get; set; }
        public Guid AuthorId { get; set; }
        public string Content { get; set; } = string.Empty;
        public CommentType CommentType { get; set; } = CommentType.General;
        public string? CodeFilePath { get; set; }
        public int? StartLine { get; set; }
        public int? EndLine { get; set; }
        public string? SuggestedCode { get; set; }
        public Guid? ParentCommentId { get; set; }
    }
}
