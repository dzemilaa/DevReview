using DevReview.Application.DTOs.Auth;
using MediatR;

namespace DevReview.Application.Users
{
    public class UpdateUserProfileCommand : IRequest<UserDto>
    {
        public string DisplayName { get; set; } = string.Empty;
        public string? Bio { get; set; }
        public string? GitHubUrl { get; set; }
        public int YearsOfExperience { get; set; }
        public decimal HourlyRate { get; set; }
        public int AvailableHoursPerWeek { get; set; }
        public System.Collections.Generic.IList<DTOs.UserLanguageDto> UserLanguages { get; set; } = new System.Collections.Generic.List<DTOs.UserLanguageDto>();
    }
}
