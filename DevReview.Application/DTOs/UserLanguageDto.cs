using DevReview.Domain.Enums;

namespace DevReview.Application.DTOs
{
    public class UserLanguageDto
    {
        public string Language { get; set; } = string.Empty;
        public ProficiencyLevel Proficiency { get; set; }
    }
}
