using System.Threading.Tasks;
using DevReview.Application.Auth;
using DevReview.Application.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevReview.API.Controllers
{
    /// <summary>Authentication — register, login, token refresh, logout.</summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ICurrentUserService _currentUserService;

        public AuthController(IMediator mediator, ICurrentUserService currentUserService)
        {
            _mediator = mediator;
            _currentUserService = currentUserService;
        }

        /// <summary>Register a new user account.</summary>
        /// <remarks>
        /// Sample request:
        /// <code>
        /// POST /api/v1/auth/register
        /// {
        ///   "userName": "jdoe",
        ///   "email": "jdoe@example.com",
        ///   "displayName": "John Doe",
        ///   "password": "Secret123!",
        ///   "role": "Author"
        /// }
        /// </code>
        /// </remarks>
        /// <response code="201">Returns access token, refresh token and user info.</response>
        /// <response code="400">Username or email already taken / validation error.</response>
        [HttpPost("register")]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Register([FromBody] RegisterCommand command)
        {
            var response = await _mediator.Send(command);
            return Created(string.Empty, response);
        }

        /// <summary>Authenticate and receive JWT tokens.</summary>
        /// <remarks>
        /// Sample request:
        /// <code>
        /// POST /api/v1/auth/login
        /// {
        ///   "usernameOrEmail": "jdoe",
        ///   "password": "Secret123!"
        /// }
        /// </code>
        /// </remarks>
        /// <response code="200">Returns access token, refresh token and user info.</response>
        /// <response code="400">Invalid credentials or account blocked.</response>
        [HttpPost("login")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Login([FromBody] LoginCommand command)
        {
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        /// <summary>Refresh an expired access token using a refresh token.</summary>
        /// <response code="200">New access token and rotated refresh token.</response>
        /// <response code="400">Invalid or expired refresh token.</response>
        [HttpPost("refresh")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenCommand command)
        {
            var response = await _mediator.Send(command);
            return Ok(response);
        }

        /// <summary>Revoke all refresh tokens for the current user (logout).</summary>
        /// <response code="204">Logged out successfully.</response>
        [Authorize(Policy = "AuthenticatedUser")]
        [HttpPost("logout")]
        [ProducesResponseType(204)]
        public async Task<IActionResult> Logout()
        {
            await _mediator.Send(new LogoutCommand());
            return NoContent();
        }

        /// <summary>Returns the current authenticated user's ID.</summary>
        /// <response code="200">User ID and authentication status.</response>
        [Authorize(Policy = "AuthenticatedUser")]
        [HttpGet("me")]
        [ProducesResponseType(200)]
        public IActionResult Me()
        {
            return Ok(new
            {
                UserId = _currentUserService.UserId,
                IsAuthenticated = _currentUserService.IsAuthenticated
            });
        }

        [Authorize(Policy = "MentorOnly")]
        [HttpGet("mentor-check")]
        public IActionResult MentorCheck() => Ok(new { Message = "Mentor access granted" });

        [Authorize(Policy = "AdminOnly")]
        [HttpGet("admin-check")]
        public IActionResult AdminCheck() => Ok(new { Message = "Admin access granted" });
    }
}
