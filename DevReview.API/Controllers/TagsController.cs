using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DevReview.Application.Tags;
using DevReview.Application.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevReview.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = "AdminOnly")]
    public class TagsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TagsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IReadOnlyList<TagDto>>> Get()
        {
            var result = await _mediator.Send(new GetTagsQuery());
            return Ok(result);
        }

        [HttpPost]
        public async Task<ActionResult<TagDto>> Create([FromBody] CreateTagCommand command)
        {
            var result = await _mediator.Send(command);
            return CreatedAtAction(nameof(Get), new { id = result.Id }, result);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<TagDto>> Update(Guid id, [FromBody] UpdateTagCommand command)
        {
            command.Id = id;
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _mediator.Send(new DeleteTagCommand { Id = id });
            return NoContent();
        }
    }
}
