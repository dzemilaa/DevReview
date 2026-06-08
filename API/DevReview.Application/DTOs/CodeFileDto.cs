using System;

namespace DevReview.Application.DTOs
{
    public class CodeFileDto
    {
        public Guid Id { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public int OrderIndex { get; set; }
    }
}
