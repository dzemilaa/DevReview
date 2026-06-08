using DevReview.Application.DTOs;
using MediatR;

namespace DevReview.Application.Admin
{
    public class GetSystemStatisticsQuery : IRequest<AdminStatisticsDto>
    {
    }
}
