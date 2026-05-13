using HomeTaste.API.Wrappers;
using HomeTaste.Application.Interfaces.Notification;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HomeTaste.API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        /// <summary>Returns paginated list of the current user's notifications.</summary>
        [HttpGet]
        public async Task<IActionResult> GetMyNotifications([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 20)
        {
            var result = await _notificationService.GetMyNotificationsAsync(pageNumber, pageSize);
            return ApiResponseMapper.FromResult(this, result);
        }

        /// <summary>Returns the unread notification count for the current user.</summary>
        [HttpGet("unread-count")]
        public async Task<IActionResult> GetUnreadCount()
        {
            var result = await _notificationService.GetUnreadCountAsync();
            return ApiResponseMapper.FromResult(this, result);
        }

        /// <summary>Marks a single notification as read.</summary>
        [HttpPatch("{id}/read")]
        public async Task<IActionResult> MarkAsRead(Guid id)
        {
            var result = await _notificationService.MarkAsReadAsync(id);
            return ApiResponseMapper.FromResult(this, result);
        }

        /// <summary>Marks all of the current user's notifications as read.</summary>
        [HttpPatch("read-all")]
        public async Task<IActionResult> MarkAllAsRead()
        {
            var result = await _notificationService.MarkAllAsReadAsync();
            return ApiResponseMapper.FromResult(this, result);
        }

        /// <summary>Deletes a notification owned by the current user.</summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNotification(Guid id)
        {
            var result = await _notificationService.DeleteNotificationAsync(id);
            return ApiResponseMapper.FromResult(this, result);
        }
    }
}
