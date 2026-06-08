using System;
using DevReview.Domain.Enums;

namespace DevReview.Domain.Entities
{
    public class UserLanguage
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid UserId { get; set; }
        public User? User { get; set; }

        public string Language { get; set; } = string.Empty;
        public ProficiencyLevel Proficiency { get; set; }
    }
}
