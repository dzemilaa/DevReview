using System.Threading.Tasks;
using DevReview.Application.Interfaces;
using DevReview.Application.Search;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DevReview.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SearchController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ICurrentUserService _currentUserService;

        public SearchController(IMediator mediator, ICurrentUserService currentUserService)
        {
            _mediator = mediator;
            _currentUserService = currentUserService;
        }

        [HttpGet("review-requests")]
        public async Task<IActionResult> SearchReviewRequests([FromQuery] SearchReviewRequestsQuery query)
        {
            query.CurrentUserId = _currentUserService.IsAuthenticated ? _currentUserService.UserId : null;
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("mentors")]
        public async Task<IActionResult> SearchMentors([FromQuery] SearchMentorsQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
}
