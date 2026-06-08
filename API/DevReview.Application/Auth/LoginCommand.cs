using DevReview.Application.DTOs.Auth;
using MediatR;

namespace DevReview.Application.Auth
{
    public class LoginCommand : IRequest<AuthResponse>
    {
        public string UsernameOrEmail { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
