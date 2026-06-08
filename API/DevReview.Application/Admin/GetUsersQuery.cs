using System.Collections.Generic;
using DevReview.Application.DTOs;
using MediatR;

namespace DevReview.Application.Admin
{
    public class GetUsersQuery : IRequest<IReadOnlyList<AdminUserDto>>
    {
    }
}
