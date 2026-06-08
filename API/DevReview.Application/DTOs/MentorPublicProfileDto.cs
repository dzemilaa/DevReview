using System;
using System.Collections.Generic;

namespace DevReview.Application.DTOs
{
    public class MentorPublicProfileDto
    {
        public Guid Id { get; set; }
        public string DisplayName { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string? Bio { get; set; }
        public string? GitHubUrl { get; set; }
        public int YearsOfExperience { get; set; }
        public decimal HourlyRate { get; set; }
        public int AvailableHoursPerWeek { get; set; }
        public decimal AverageMentorRating { get; set; }
        public int TotalReviews { get; set; }
        public IList<LanguageExpertiseDto> LanguagesExpertise { get; set; } = new List<LanguageExpertiseDto>();
        public IList<string> ProfileLanguages { get; set; } = new List<string>();
        public IList<ReviewRequestDto> PublicReviews { get; set; } = new List<ReviewRequestDto>();
    }
}
