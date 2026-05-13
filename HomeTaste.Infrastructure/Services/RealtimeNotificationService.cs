using HomeTaste.Application.DTOs.Notification;
using HomeTaste.Application.Interfaces.Realtime;
using HomeTaste.Infrastructure.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace HomeTaste.Infrastructure.Services
{
    public class RealtimeNotificationService : IRealtimeNotificationService
    {
        private readonly IHubContext<NotificationHub> _hubContext;

        public RealtimeNotificationService(IHubContext<NotificationHub> hubContext)
        {
            _hubContext = hubContext;
        }

        /// <summary>Pushes a new notification to the target user's connected clients.</summary>
        public async Task SendNotificationAsync(string userId, NotificationResponse notification)
        {
            await _hubContext.Clients
                .Group($"user_{userId}")
                .SendAsync("ReceiveNotification", notification);
        }

        /// <summary>Pushes an order status change event to the target user.</summary>
        public async Task SendOrderStatusUpdateAsync(string userId, Guid orderId, string status)
        {
            await _hubContext.Clients
                .Group($"user_{userId}")
                .SendAsync("OrderStatusUpdated", new { orderId, status, timestamp = DateTime.UtcNow });
        }

        /// <summary>Pushes a delivery personnel GPS location update to the customer.</summary>
        public async Task SendDeliveryLocationUpdateAsync(string userId, Guid assignmentId, double latitude, double longitude)
        {
            await _hubContext.Clients
                .Group($"user_{userId}")
                .SendAsync("DeliveryLocationUpdated", new { assignmentId, latitude, longitude, timestamp = DateTime.UtcNow });
        }
    }
}
