using System.Threading.Tasks;
using DevReview.Application.Users;
using DevReview.Application.DTOs.Auth;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevReview.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = "AuthenticatedUser")]
    public class ProfilesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProfilesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("me")]
        public async Task<ActionResult<UserDto>> GetCurrentProfile()
        {
            var result = await _mediator.Send(new GetUserProfileQuery());
            return Ok(result);
        }

        [HttpPut("me")]
        public async Task<ActionResult<UserDto>> UpdateCurrentProfile([FromBody] UpdateUserProfileCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }
    }
}
