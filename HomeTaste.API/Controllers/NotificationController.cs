using HomeTaste.Application.Features.Notifications.Commands.DeleteNotification;
using HomeTaste.Application.Features.Notifications.Commands.MarkAllAsRead;
using HomeTaste.Application.Features.Notifications.Commands.MarkAsRead;
using HomeTaste.Application.Features.Notifications.Queries.GetMyNotifications;
using HomeTaste.Application.Features.Notifications.Queries.GetUnreadCount;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HomeTaste.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly IMediator _mediator;

        public NotificationController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>Returns paginated list of the current user's notifications.</summary>
        [HttpGet]
        public async Task<IActionResult> GetMyNotifications([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20)
        {
            var result = await _mediator.Send(new GetMyNotificationsQuery { PageNumber = pageNumber, PageSize = pageSize });
            return Ok(result);
        }

        /// <summary>Returns the unread notification count for the current user.</summary>
        [HttpGet("unread-count")]
        public async Task<IActionResult> GetUnreadCount()
        {
            var result = await _mediator.Send(new GetUnreadCountQuery());
            return Ok(result);
        }

        /// <summary>Marks a single notification as read.</summary>
        [HttpPatch("{id}/read")]
        public async Task<IActionResult> MarkAsRead(Guid id)
        {
            var result = await _mediator.Send(new MarkAsReadCommand(id));
            return Ok(result);
        }

        /// <summary>Marks all of the current user's notifications as read.</summary>
        [HttpPatch("read-all")]
        public async Task<IActionResult> MarkAllAsRead()
        {
            var result = await _mediator.Send(new MarkAllAsReadCommand());
            return Ok(result);
        }

        /// <summary>Deletes a notification owned by the current user.</summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNotification(Guid id)
        {
            var result = await _mediator.Send(new DeleteNotificationCommand(id));
            return Ok(result);
        }
    }
}
