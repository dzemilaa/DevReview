using System;
using System.Collections.Generic;
using DevReview.Application.DTOs;
using MediatR;

namespace DevReview.Application.OfficeHours
{
    public class GetAvailableOfficeHoursQuery : IRequest<IReadOnlyList<OfficeHourDetailDto>>
    {
        public Guid? CurrentUserId { get; set; }
    }
}
