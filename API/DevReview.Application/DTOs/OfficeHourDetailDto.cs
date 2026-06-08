using System;
using DevReview.Domain.Enums;

namespace DevReview.Application.DTOs
{
    public class OfficeHourDetailDto : OfficeHourDto
    {
        public string MentorName { get; set; } = string.Empty;
        public string MentorEmail { get; set; } = string.Empty;
        public string? BookedByName { get; set; }
        public string? BookedByEmail { get; set; }
        public string? MentorImpression { get; set; }
        public string? AuthorImpression { get; set; }
    }
}
