using System;
using DevReview.Domain.Enums;

namespace DevReview.Application.DTOs
{
    public class OfficeHourDto
    {
        public Guid Id { get; set; }
        public Guid MentorId { get; set; }
        public DateTime StartTime { get; set; }
        public int DurationMinutes { get; set; }
        public DateTime EndTime { get; set; }
        public string Topic { get; set; } = string.Empty;
        public decimal? Price { get; set; }
        public OfficeHourStatus Status { get; set; }
        public Guid? BookedById { get; set; }
        public string? BookingDescription { get; set; }
    }
}
