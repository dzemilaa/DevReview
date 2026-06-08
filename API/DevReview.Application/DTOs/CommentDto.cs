using System;
using DevReview.Domain.Enums;

namespace DevReview.Application.DTOs
{
    public class CommentDto
    {
        public Guid Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public CommentType Type { get; set; }
        public int? StartLine { get; set; }
        public int? EndLine { get; set; }
        public string? SuggestedCode { get; set; }
        public bool IsResolved { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid UserId { get; set; }
        public string AuthorName { get; set; } = string.Empty;
        public Guid? CodeFileId { get; set; }
        public string? CodeFilePath { get; set; }
        public Guid? ParentCommentId { get; set; }
        public IList<CommentDto> Replies { get; set; } = new List<CommentDto>();
    }
}
