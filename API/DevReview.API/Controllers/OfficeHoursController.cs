using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DevReview.Application.Interfaces;
using DevReview.Application.OfficeHours;
using DevReview.Application.DTOs;
using DevReview.Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevReview.API.Controllers
{
    /// <summary>Mentoring sessions (Office Hours) — create slots, book, cancel, submit feedback.</summary>
    [ApiController]
    [Route("api/[controller]")]
    public class OfficeHoursController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ICurrentUserService _currentUserService;

        public OfficeHoursController(IMediator mediator, ICurrentUserService currentUserService)
        {
            _mediator = mediator;
            _currentUserService = currentUserService;
        }

        [Authorize]
        [HttpGet("my-bookings")]
        public async Task<ActionResult<IReadOnlyList<OfficeHourDetailDto>>> GetMyBookings()
        {
            var result = await _mediator.Send(new GetMyBookedOfficeHoursQuery());
            return Ok(result);
        }

        [HttpGet("available")]
        public async Task<ActionResult<IReadOnlyList<OfficeHourDetailDto>>> GetAvailable()
        {
            var userId = _currentUserService.IsAuthenticated ? _currentUserService.UserId : (Guid?)null;
            var result = await _mediator.Send(new GetAvailableOfficeHoursQuery { CurrentUserId = userId });
            return Ok(result);
        }

        [Authorize(Roles = UserRoles.MentorOrAdmin)]
        [HttpGet("mentor/schedule")]
        public async Task<ActionResult<IReadOnlyList<OfficeHourDetailDto>>> GetMentorSchedule()
        {
            var result = await _mediator.Send(new GetMentorScheduleQuery { MentorId = _currentUserService.UserId });
            return Ok(result);
        }

        [Authorize(Roles = UserRoles.MentorOrAdmin)]
        [HttpPost]
        public async Task<ActionResult<OfficeHourDto>> Create([FromBody] CreateOfficeHourCommand command)
        {
            var result = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetAvailable), new { id = result.Id }, result);
        }

        [Authorize]
        [HttpPost("{id}/book")]
        public async Task<ActionResult<OfficeHourDto>> Book(Guid id, [FromBody] BookOfficeHourCommand command)
        {
            command.OfficeHourId = id;
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [Authorize]
        [HttpPost("{id}/cancel")]
        public async Task<ActionResult<OfficeHourDto>> Cancel(Guid id, [FromBody] CancelOfficeHourCommand command)
        {
            command.OfficeHourId = id;
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [Authorize]
        [HttpPost("{id}/feedback")]
        public async Task<ActionResult<OfficeHourDetailDto>> SubmitFeedback(Guid id, [FromBody] SubmitOfficeHourFeedbackCommand command)
        {
            command.OfficeHourId = id;
            var result = await _mediator.Send(command);
            return Ok(result);
        }

    }
}
