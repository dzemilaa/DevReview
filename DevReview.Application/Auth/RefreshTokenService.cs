using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DevReview.Application.Interfaces;
using DevReview.Domain.Entities;

namespace DevReview.Application.Auth
{
    public static class RefreshTokenService
    {
        public static async Task<string> IssueAsync(
            IUnitOfWork unitOfWork,
            IJwtTokenService tokenService,
            User user,
            int expirationDays,
            CancellationToken cancellationToken)
        {
            var activeTokens = await unitOfWork.UserRefreshTokens.FindAsync(
                t => t.UserId == user.Id && !t.IsRevoked,
                cancellationToken);

            foreach (var active in activeTokens)
            {
                active.IsRevoked = true;
                unitOfWork.UserRefreshTokens.Update(active);
            }

            var refreshToken = tokenService.CreateRefreshToken();
            var expiresAt = DateTime.UtcNow.AddDays(expirationDays);

            await unitOfWork.UserRefreshTokens.AddAsync(new UserRefreshToken
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                Token = refreshToken,
                ExpiresAt = expiresAt,
                IsRevoked = false
            }, cancellationToken);

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiry = expiresAt;
            unitOfWork.Users.Update(user);

            return refreshToken;
        }

        public static async Task<User> ValidateAndGetUserAsync(
            IUnitOfWork unitOfWork,
            string refreshToken,
            CancellationToken cancellationToken)
        {
            var stored = (await unitOfWork.UserRefreshTokens.FindAsync(
                t => t.Token == refreshToken && !t.IsRevoked,
                cancellationToken)).FirstOrDefault();

            if (stored == null || stored.ExpiresAt < DateTime.UtcNow)
            {
                throw new ApplicationException("Invalid or expired refresh token.");
            }

            var user = await unitOfWork.Users.GetByIdAsync(stored.UserId, cancellationToken)
                ?? throw new ApplicationException("User not found.");

            stored.IsRevoked = true;
            unitOfWork.UserRefreshTokens.Update(stored);

            return user;
        }

        public static async Task RevokeAllForUserAsync(
            IUnitOfWork unitOfWork,
            Guid userId,
            CancellationToken cancellationToken)
        {
            var tokens = await unitOfWork.UserRefreshTokens.FindAsync(
                t => t.UserId == userId && !t.IsRevoked,
                cancellationToken);

            foreach (var token in tokens)
            {
                token.IsRevoked = true;
                unitOfWork.UserRefreshTokens.Update(token);
            }

            var user = await unitOfWork.Users.GetByIdAsync(userId, cancellationToken);
            if (user != null)
            {
                user.RefreshToken = null;
                user.RefreshTokenExpiry = null;
                unitOfWork.Users.Update(user);
            }
        }
    }
}
