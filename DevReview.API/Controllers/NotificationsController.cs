using System.Threading.Tasks;
using DevReview.Application.Notifications;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevReview.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Policy = "AuthenticatedUser")]
    public class NotificationsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public NotificationsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] bool onlyUnread = false)
        {
            var result = await _mediator.Send(new GetNotificationsQuery { OnlyUnread = onlyUnread });
            return Ok(result);
        }

        [HttpPost("{notificationId}/read")]
        public async Task<IActionResult> MarkRead(System.Guid notificationId)
        {
            await _mediator.Send(new MarkNotificationReadCommand { NotificationId = notificationId });
            return NoContent();
        }

        [HttpPost("read-all")]
        public async Task<IActionResult> MarkAllRead()
        {
            await _mediator.Send(new MarkAllNotificationsReadCommand());
            return NoContent();
        }
    }
}
