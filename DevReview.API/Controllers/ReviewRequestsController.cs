using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DevReview.API.Services;
using DevReview.Application.Interfaces;
using DevReview.Application.ReviewRequests;
using DevReview.Application.DTOs;
using DevReview.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace DevReview.API.Controllers
{
    /// <summary>Manages code review requests — lifecycle, comments, suggestions, finalization.</summary>
    [ApiController]
    [Route("api/[controller]")]
    [Route("api/review-requests")]
    [Authorize(Policy = "AuthenticatedUser")]
    public class ReviewRequestsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ICurrentUserService _currentUserService;

        public ReviewRequestsController(IMediator mediator, ICurrentUserService currentUserService)
        {
            _mediator = mediator;
            _currentUserService = currentUserService;
        }

        [AllowAnonymous]
        [HttpGet("public")]
        public async Task<ActionResult<IReadOnlyList<ReviewRequestDto>>> GetPublic(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var result = await _mediator.Send(new GetPublicReviewRequestsQuery
            {
                Page = page,
                PageSize = pageSize
            });
            return Ok(result);
        }

        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<ReviewRequestDto>>> Get()
        {
            var result = await _mediator.Send(new GetReviewRequestsQuery());
            return Ok(result);
        }

        [HttpGet("my")]
        public async Task<ActionResult<IReadOnlyList<ReviewRequestDto>>> GetMine()
        {
            var result = await _mediator.Send(new GetReviewRequestsQuery { OwnerId = _currentUserService.UserId });
            return Ok(result);
        }

        [Authorize(Roles = UserRoles.MentorOrAdmin)]
        [HttpGet("claimed-by-me")]
        public async Task<ActionResult<IReadOnlyList<ReviewRequestDto>>> GetClaimedByMe()
        {
            var result = await _mediator.Send(new GetReviewRequestsQuery { MentorId = _currentUserService.UserId });
            return Ok(result);
        }

        [AllowAnonymous]
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<ReviewRequestDetailDto>> Get(Guid id)
        {
            var result = await _mediator.Send(new GetReviewRequestByIdQuery
            {
                ReviewRequestId = id,
                RequirePublic = !_currentUserService.IsAuthenticated
            });
            return Ok(result);
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<ReviewRequestDto>> Create([FromBody] CreateReviewRequestCommand command)
        {
            command.OwnerId = _currentUserService.UserId;
            var result = await _mediator.Send(command);
            return CreatedAtAction(nameof(Get), new { id = result.Id }, result);
        }

        [Authorize(Roles = UserRoles.MentorOrAdmin)]
        [HttpPost("{id}/claim")]
        public async Task<ActionResult<ReviewRequestDto>> Claim(Guid id)
        {
            var command = new ClaimReviewRequestCommand
            {
                ReviewRequestId = id,
                MentorId = _currentUserService.UserId
            };

            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [Authorize(Roles = UserRoles.MentorOrAdmin)]
        [HttpPost("{id}/abandon")]
        public async Task<ActionResult<ReviewRequestDto>> Abandon(Guid id, [FromBody] AbandonReviewRequestCommand command)
        {
            command.ReviewRequestId = id;
            command.MentorId = _currentUserService.UserId;

            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPost("parse-zip")]
        [RequestSizeLimit(5 * 1024 * 1024)]
        public ActionResult<IList<CodeFileDto>> ParseZip(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest("ZIP file is required.");
            }

            if (file.Length > 5 * 1024 * 1024)
            {
                return BadRequest("ZIP file must be 5 MB or smaller.");
            }

            if (!file.FileName.EndsWith(".zip", StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest("Only .zip archives are supported.");
            }

            using var stream = file.OpenReadStream();
            var files = CodeZipParser.Parse(stream);
            return Ok(files);
        }

        [HttpPost("{id}/comments")]
        public async Task<ActionResult<CommentDto>> AddComment(Guid id, [FromBody] AddCommentCommand command)
        {
            command.ReviewRequestId = id;
            command.AuthorId = _currentUserService.UserId;

            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [Authorize(Roles = UserRoles.MentorOrAdmin)]
        [HttpPost("{id}/comments/{commentId}/resolve")]
        public async Task<ActionResult<CommentDto>> ResolveComment(Guid id, Guid commentId)
        {
            var command = new ResolveCommentCommand
            {
                ReviewRequestId = id,
                CommentId = commentId,
                MentorId = _currentUserService.UserId
            };

            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPost("{id}/rate")]
        public async Task<IActionResult> SubmitPeerRating(Guid id, [FromBody] SubmitPeerRatingCommand command)
        {
            command.ReviewRequestId = id;
            await _mediator.Send(command);
            return NoContent();
        }

        [HttpPost("{id}/comments/{commentId}/apply")]
        public async Task<ActionResult<CodeFileDto>> ApplySuggestion(Guid id, Guid commentId)
        {
            var command = new ApplySuggestionCommand
            {
                ReviewRequestId = id,
                CommentId = commentId,
                AuthorId = _currentUserService.UserId
            };

            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [Authorize(Roles = UserRoles.MentorOrAdmin)]
        [HttpPost("{id}/finalize")]
        public async Task<ActionResult<ReviewRequestDto>> SubmitFinalReview(Guid id, [FromBody] SubmitFinalReviewCommand command)
        {
            command.ReviewRequestId = id;
            command.MentorId = _currentUserService.UserId;

            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpPost("{id}/approve")]
        public async Task<IActionResult> ApproveReview(Guid id, [FromBody] bool approved)
        {
            await _mediator.Send(new ApproveReviewCommand
            {
                ReviewRequestId = id,
                AuthorId = _currentUserService.UserId,
                Approved = approved
            });
            return NoContent();
        }
    }
}
