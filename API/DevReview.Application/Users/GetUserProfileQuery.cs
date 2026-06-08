using DevReview.Application.DTOs.Auth;
using MediatR;

namespace DevReview.Application.Users
{
    public class GetUserProfileQuery : IRequest<UserDto>
    {
    }
}
