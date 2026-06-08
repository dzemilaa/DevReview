using System;
using DevReview.Domain.Enums;

namespace DevReview.Domain.Entities
{
    public class OfficeHour
    {
        public Guid Id { get; set; }
        public Guid MentorId { get; set; }
        public User? Mentor { get; set; }
        public DateTime StartTime { get; set; }
        public int DurationMinutes { get; set; }
        public DateTime EndTime { get; set; }
        public string Topic { get; set; } = string.Empty;
        public decimal? Price { get; set; }
        public OfficeHourStatus Status { get; set; } = OfficeHourStatus.Available;
        public Guid? BookedById { get; set; }
        public User? BookedBy { get; set; }
        public string? BookingDescription { get; set; }
        public string? MentorImpression { get; set; }
        public string? AuthorImpression { get; set; }
    }
}
