using System;

namespace DevReview.Domain.Entities
{
    public class CodeFile
    {
        public Guid Id { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public int OrderIndex { get; set; }

        public Guid ReviewRequestId { get; set; }
        public ReviewRequest? ReviewRequest { get; set; }
    }
}
