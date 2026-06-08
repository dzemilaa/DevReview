using System;
using DevReview.Application.DTOs.Auth;
using MediatR;

namespace DevReview.Application.Auth
{
    public class RegisterCommand : IRequest<AuthResponse>
    {
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }
}
