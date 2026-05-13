using HomeTaste.Application.DTOs.Notification;

namespace HomeTaste.Application.Interfaces.Realtime
{
    public interface IRealtimeNotificationService
    {
        Task SendNotificationAsync(string userId, NotificationResponse notification);
        Task SendOrderStatusUpdateAsync(string userId, Guid orderId, string status);
        Task SendDeliveryLocationUpdateAsync(string userId, Guid assignmentId, double latitude, double longitude);
    }
}
