using DevReview.Application.DTOs.Auth;
using MediatR;

namespace DevReview.Application.Auth
{
    public class RefreshTokenCommand : IRequest<AuthResponse>
    {
        public string RefreshToken { get; set; } = string.Empty;
    }
}
