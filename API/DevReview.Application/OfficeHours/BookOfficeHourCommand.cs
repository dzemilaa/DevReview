using System;
using DevReview.Application.DTOs;
using MediatR;

namespace DevReview.Application.OfficeHours
{
    public class BookOfficeHourCommand : IRequest<OfficeHourDto>
    {
        public Guid OfficeHourId { get; set; }
        public string? BookingDescription { get; set; }
    }
}
