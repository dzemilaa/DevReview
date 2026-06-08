using System;
using System.Collections.Generic;
using DevReview.Domain.Enums;

namespace DevReview.Domain.Entities
{
    public class User
    {
        public Guid Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string? Bio { get; set; }
        public string? GitHubUrl { get; set; }
        public int AvailableHoursPerWeek { get; set; }
        public string PasswordHash { get; set; } = string.Empty;
        public string Role { get; set; } = UserRoles.Author;
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiry { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsBlocked { get; set; }
        public DateTime? BlockedAt { get; set; }
        public string? BlockReason { get; set; }
        public int YearsOfExperience { get; set; }
        public decimal HourlyRate { get; set; }
        public decimal AverageMentorRating { get; set; }
        public decimal AverageAuthorRating { get; set; }
        public int TotalReviews { get; set; }
        public int TotalAuthorRatings { get; set; }

        public ICollection<ReviewRequest> ReviewRequests { get; set; } = new List<ReviewRequest>();
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public ICollection<OfficeHour> OfficeHours { get; set; } = new List<OfficeHour>();
        public ICollection<MentorReputation> MentorReputations { get; set; } = new List<MentorReputation>();
        public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
        public ICollection<UserLanguage> UserLanguages { get; set; } = new List<UserLanguage>();
    }
}
