using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DevReview.Application.Mentors;
using DevReview.Application.DTOs;
using DevReview.Application.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevReview.API.Controllers
{
    /// <summary>Mentor rankings, search, and follow/unfollow.</summary>
    [ApiController]
    [Route("api/[controller]")]
    public class MentorsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ICurrentUserService _currentUserService;

        public MentorsController(IMediator mediator, ICurrentUserService currentUserService)
        {
            _mediator = mediator;
            _currentUserService = currentUserService;
        }

        [HttpGet("{mentorId:guid}")]
        public async Task<IActionResult> GetProfile(Guid mentorId)
        {
            var result = await _mediator.Send(new GetMentorPublicProfileQuery { MentorId = mentorId });
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpGet("top")]
        public async Task<ActionResult<IReadOnlyList<MentorRankingDto>>> GetTop(
            [FromQuery] string? language,
            [FromQuery] decimal? minimumRating,
            [FromQuery] int? minimumYearsOfExperience,
            [FromQuery] string? period)
        {
            var result = await _mediator.Send(new GetTopMentorsQuery
            {
                Language = language,
                MinimumRating = minimumRating,
                MinimumYearsOfExperience = minimumYearsOfExperience,
                Period = period
            });
            return Ok(result);
        }

        [Authorize]
        [HttpPost("{mentorId}/follow")]
        public async Task<IActionResult> Follow(Guid mentorId)
        {
            await _mediator.Send(new FollowMentorCommand
            {
                MentorId = mentorId,
                FollowerId = _currentUserService.UserId
            });
            return NoContent();
        }

        [Authorize]
        [HttpDelete("{mentorId}/follow")]
        public async Task<IActionResult> Unfollow(Guid mentorId)
        {
            await _mediator.Send(new UnfollowMentorCommand
            {
                MentorId = mentorId,
                FollowerId = _currentUserService.UserId
            });
            return NoContent();
        }

        [Authorize]
        [HttpGet("following")]
        public async Task<ActionResult<IReadOnlyList<Guid>>> GetFollowing()
        {
            var result = await _mediator.Send(new GetFollowedMentorIdsQuery
            {
                FollowerId = _currentUserService.UserId
            });
            return Ok(result);
        }

        [Authorize]
        [HttpGet("following/details")]
        public async Task<ActionResult<IReadOnlyList<MentorRankingDto>>> GetFollowingDetails()
        {
            var result = await _mediator.Send(new GetFollowedMentorsQuery
            {
                FollowerId = _currentUserService.UserId
            });
            return Ok(result);
        }
    }
}
