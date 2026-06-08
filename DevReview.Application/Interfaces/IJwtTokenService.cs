using System;
using DevReview.Domain.Entities;

namespace DevReview.Application.Interfaces
{
    public interface IJwtTokenService
    {
        string CreateToken(User user);
        string CreateRefreshToken();
        DateTime GetTokenExpiration();
    }
}
