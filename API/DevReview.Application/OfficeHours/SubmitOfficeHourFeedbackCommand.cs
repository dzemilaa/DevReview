using System;
using DevReview.Application.DTOs;
using MediatR;

namespace DevReview.Application.OfficeHours
{
    public class SubmitOfficeHourFeedbackCommand : IRequest<OfficeHourDetailDto>
    {
        public Guid OfficeHourId { get; set; }
        public string Impression { get; set; } = string.Empty;
    }
}
