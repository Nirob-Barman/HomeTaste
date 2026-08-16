using HomeTaste.Application.Features.Notifications;
using HomeTaste.Application.Interfaces.Notification;
using HomeTaste.Application.Interfaces.Persistence;
using HomeTaste.Application.Interfaces.Realtime;
using HomeTaste.Domain.Enums;
using NotificationEntity = HomeTaste.Domain.Entities.Notification.Notification;

namespace HomeTaste.Application.Services.Notification
{
    public class NotificationService : INotificationService
    {
        private readonly IApplicationDbContext _context;
        private readonly IRealtimeNotificationService _realtimeService;

        public NotificationService(IApplicationDbContext context, IRealtimeNotificationService realtimeService)
        {
            _context = context;
            _realtimeService = realtimeService;
        }

        public async Task CreateNotificationAsync(
            string userId,
            string title,
            string message,
            NotificationType type,
            Guid? referenceId = null,
            string? referenceType = null)
        {
            var notification = NotificationEntity.Create(userId, title, message, type, referenceId, referenceType);

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();

            // Push real-time notification — fire-and-forget; failures are non-critical
            try
            {
                await _realtimeService.SendNotificationAsync(userId, NotificationMapper.ToResponse(notification));
            }
            catch
            {
                // Real-time push is best-effort; DB record is already persisted
            }
        }
    }
}
