using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using DevReview.Application.Interfaces;
using Microsoft.AspNetCore.Http;

namespace DevReview.API.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public Guid UserId
        {
            get
            {
                var context = _httpContextAccessor.HttpContext;
                if (context?.User?.Identity?.IsAuthenticated != true)
                {
                    return Guid.Empty;
                }

                var userIdClaim = context.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? context.User.FindFirstValue(JwtRegisteredClaimNames.Sub);
                return Guid.TryParse(userIdClaim, out var userId) ? userId : Guid.Empty;
            }
        }

        public bool IsAuthenticated => _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;

        public string? Role
        {
            get
            {
                var context = _httpContextAccessor.HttpContext;
                if (context?.User?.Identity?.IsAuthenticated != true)
                {
                    return null;
                }

                return context.User.FindFirstValue(ClaimTypes.Role);
            }
        }
    }
}
