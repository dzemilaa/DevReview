using System;
using System.Collections.Generic;

namespace DevReview.Application.DTOs
{
    public class MentorRankingDto
    {
        public Guid MentorId { get; set; }
        public string DisplayName { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string? Bio { get; set; }
        public decimal OverallRating { get; set; }
        public int TotalReviews { get; set; }
        public int YearsOfExperience { get; set; }
        public decimal? HourlyRate { get; set; }
        public int AvailableHoursPerWeek { get; set; }
        public IList<LanguageExpertiseDto> LanguagesExpertise { get; set; } = new List<LanguageExpertiseDto>();
        public IList<string> ProfileLanguages { get; set; } = new List<string>();
    }
}
