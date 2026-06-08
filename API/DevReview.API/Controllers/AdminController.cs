using System.Threading.Tasks;
using DevReview.Application.Admin;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevReview.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = "AdminOnly")]
    public class AdminController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AdminController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("users")]
        public async Task<IActionResult> GetUsers()
        {
            var result = await _mediator.Send(new GetUsersQuery());
            return Ok(result);
        }

        [HttpPost("users/{userId}/block")]
        public async Task<IActionResult> BlockUser(System.Guid userId, [FromBody] BlockUserCommand command)
        {
            command.UserId = userId;
            await _mediator.Send(command);
            return NoContent();
        }

        [HttpPost("users/{userId}/unblock")]
        public async Task<IActionResult> UnblockUser(System.Guid userId)
        {
            await _mediator.Send(new UnblockUserCommand { UserId = userId });
            return NoContent();
        }

        [HttpGet("statistics")]
        public async Task<IActionResult> GetStatistics()
        {
            var result = await _mediator.Send(new GetSystemStatisticsQuery());
            return Ok(result);
        }

        [HttpDelete("review-requests/{reviewRequestId}")]
        public async Task<IActionResult> DeleteReviewRequest(System.Guid reviewRequestId)
        {
            await _mediator.Send(new DeleteReviewRequestCommand { ReviewRequestId = reviewRequestId });
            return NoContent();
        }

        [HttpDelete("comments/{commentId}")]
        public async Task<IActionResult> DeleteComment(System.Guid commentId)
        {
            await _mediator.Send(new DeleteCommentCommand { CommentId = commentId });
            return NoContent();
        }
    }
}
