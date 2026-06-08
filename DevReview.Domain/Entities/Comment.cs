using System;
using DevReview.Domain.Enums;

namespace DevReview.Domain.Entities
{
    public class Comment
    {
        public Guid Id { get; set; }
        public string Content { get; set; } = string.Empty;
        public CommentType Type { get; set; } = CommentType.Suggestion;
        public int? StartLine { get; set; }
        public int? EndLine { get; set; }
        public string? SuggestedCode { get; set; }
        public bool IsResolved { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Guid UserId { get; set; }
        public User? User { get; set; }

        public Guid ReviewRequestId { get; set; }
        public ReviewRequest? ReviewRequest { get; set; }

        public Guid? CodeFileId { get; set; }
        public CodeFile? CodeFile { get; set; }

        public Guid? ParentCommentId { get; set; }
        public Comment? ParentComment { get; set; }
        public ICollection<Comment> Replies { get; set; } = new List<Comment>();
    }
}
