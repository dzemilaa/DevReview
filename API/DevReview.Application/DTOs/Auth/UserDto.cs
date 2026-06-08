using System;
using System.Collections.Generic;

namespace DevReview.Application.DTOs.Auth
{
    public class UserDto
    {
        public Guid Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string? Bio { get; set; }
        public string? GitHubUrl { get; set; }
        public string Role { get; set; } = string.Empty;
        public int AvailableHoursPerWeek { get; set; }
        public int YearsOfExperience { get; set; }
        public decimal HourlyRate { get; set; }
        public decimal AverageMentorRating { get; set; }
        public int TotalReviews { get; set; }
        public IList<LanguageExpertiseDto> LanguagesExpertise { get; set; } = new List<LanguageExpertiseDto>();
        public IList<UserLanguageDto> UserLanguages { get; set; } = new List<UserLanguageDto>();
    }
}
