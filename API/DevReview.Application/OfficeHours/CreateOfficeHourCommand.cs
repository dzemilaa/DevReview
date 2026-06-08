using System;
using DevReview.Application.DTOs;
using MediatR;

namespace DevReview.Application.OfficeHours
{
    public class CreateOfficeHourCommand : IRequest<OfficeHourDto>
    {
        public DateTime StartTime { get; set; }
        public int DurationMinutes { get; set; }
        public string Topic { get; set; } = string.Empty;
        public decimal? Price { get; set; }
    }
}
