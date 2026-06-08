using DevReview.Application.DTOs;
using MediatR;

namespace DevReview.Application.OfficeHours
{
    public class GetMyBookedOfficeHoursQuery : IRequest<IReadOnlyList<OfficeHourDetailDto>>
    {
    }
}
