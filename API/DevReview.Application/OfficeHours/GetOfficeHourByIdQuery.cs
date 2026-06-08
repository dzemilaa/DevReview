using System;
using DevReview.Application.DTOs;
using MediatR;

namespace DevReview.Application.OfficeHours
{
    public class GetOfficeHourByIdQuery : IRequest<OfficeHourDetailDto?>
    {
        public Guid OfficeHourId { get; set; }
        public Guid MentorId { get; set; }
    }
}
