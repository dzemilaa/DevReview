using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using DevReview.Application.DTOs.Auth;
using DevReview.Application.Interfaces;
using DevReview.Application.Common;
using DevReview.Domain.Entities;
using DevReview.Domain.Enums;
using MediatR;

namespace DevReview.Application.Auth
{
    public class RegisterCommandHandler : IRequestHandler<RegisterCommand, AuthResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IJwtTokenService _tokenService;
        private readonly IMapper _mapper;

        public RegisterCommandHandler(IUnitOfWork unitOfWork, IJwtTokenService tokenService, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _tokenService = tokenService;
            _mapper = mapper;
        }

        public async Task<AuthResponse> Handle(RegisterCommand request, CancellationToken cancellationToken)
        {
            var existing = await _unitOfWork.Users.FindAsync(x => x.UserName == request.UserName || x.Email == request.Email, cancellationToken);
            if (existing.Any())
            {
                throw new ApplicationException("The username or email is already taken.");
            }

            var user = new User
            {
                Id = Guid.NewGuid(),
                UserName = request.UserName,
                Email = request.Email,
                DisplayName = request.DisplayName,
                PasswordHash = PasswordHasher.Hash(request.Password),
                Role = request.Role
            };

            await _unitOfWork.Users.AddAsync(user, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var refreshToken = await RefreshTokenService.IssueAsync(_unitOfWork, _tokenService, user, 7, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new AuthResponse
            {
                User = _mapper.Map<UserDto>(user),
                AccessToken = _tokenService.CreateToken(user),
                RefreshToken = refreshToken,
                ExpiresAt = _tokenService.GetTokenExpiration()
            };
        }
    }

    public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IJwtTokenService _tokenService;
        private readonly IMapper _mapper;

        public LoginCommandHandler(IUnitOfWork unitOfWork, IJwtTokenService tokenService, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _tokenService = tokenService;
            _mapper = mapper;
        }

        public async Task<AuthResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            var users = await _unitOfWork.Users.FindAsync(x => x.UserName == request.UsernameOrEmail || x.Email == request.UsernameOrEmail, cancellationToken);
            var user = users.FirstOrDefault();

            if (user == null || !PasswordHasher.Verify(request.Password, user.PasswordHash))
            {
                throw new ApplicationException("Invalid credentials.");
            }

            if (user.IsBlocked)
            {
                throw new ApplicationException("Account has been blocked. Please contact support.");
            }

            var refreshToken = await RefreshTokenService.IssueAsync(_unitOfWork, _tokenService, user, 7, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new AuthResponse
            {
                User = _mapper.Map<UserDto>(user),
                AccessToken = _tokenService.CreateToken(user),
                RefreshToken = refreshToken,
                ExpiresAt = _tokenService.GetTokenExpiration()
            };
        }
    }

    public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, AuthResponse>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IJwtTokenService _tokenService;
        private readonly IMapper _mapper;

        public RefreshTokenCommandHandler(IUnitOfWork unitOfWork, IJwtTokenService tokenService, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _tokenService = tokenService;
            _mapper = mapper;
        }

        public async Task<AuthResponse> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            var user = await RefreshTokenService.ValidateAndGetUserAsync(
                _unitOfWork,
                request.RefreshToken,
                cancellationToken);

            if (user.IsBlocked)
            {
                throw new ApplicationException("Account has been blocked. Please contact support.");
            }

            var refreshToken = await RefreshTokenService.IssueAsync(_unitOfWork, _tokenService, user, 7, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new AuthResponse
            {
                User = _mapper.Map<UserDto>(user),
                AccessToken = _tokenService.CreateToken(user),
                RefreshToken = refreshToken,
                ExpiresAt = _tokenService.GetTokenExpiration()
            };
        }
    }

    public class LogoutCommandHandler : IRequestHandler<LogoutCommand, Unit>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICurrentUserService _currentUserService;

        public LogoutCommandHandler(IUnitOfWork unitOfWork, ICurrentUserService currentUserService)
        {
            _unitOfWork = unitOfWork;
            _currentUserService = currentUserService;
        }

        public async Task<Unit> Handle(LogoutCommand request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;
            if (userId == Guid.Empty)
            {
                throw new ApplicationException("User is not authenticated.");
            }

            var user = await _unitOfWork.Users.GetByIdAsync(userId, cancellationToken);
            if (user == null)
            {
                throw new ApplicationException("User not found.");
            }

            await RefreshTokenService.RevokeAllForUserAsync(_unitOfWork, userId, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}
