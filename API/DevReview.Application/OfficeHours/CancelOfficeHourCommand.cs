using System;
using DevReview.Application.DTOs;
using MediatR;

namespace DevReview.Application.OfficeHours
{
    public class CancelOfficeHourCommand : IRequest<OfficeHourDto>
    {
        public Guid OfficeHourId { get; set; }
        public string? CancellationReason { get; set; }
    }
}
