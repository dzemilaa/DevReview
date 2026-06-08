using System;
using System.Collections.Generic;
using DevReview.Application.DTOs;
using MediatR;

namespace DevReview.Application.OfficeHours
{
    public class GetMentorScheduleQuery : IRequest<IReadOnlyList<OfficeHourDetailDto>>
    {
        public Guid MentorId { get; set; }
    }
}
