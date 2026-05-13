using HomeTaste.Application.DTOs.Notification;
using HomeTaste.Application.Wrappers;
using HomeTaste.Domain.Enums;

namespace HomeTaste.Application.Interfaces.Notification
{
    public interface INotificationService
    {
        Task<Result<PaginatedResponse<IEnumerable<NotificationResponse>>>> GetMyNotificationsAsync(int pageNumber = 1, int pageSize = 20);
        Task<Result<UnreadCountResponse>> GetUnreadCountAsync();
        Task<Result<bool>> MarkAsReadAsync(Guid id);
        Task<Result<bool>> MarkAllAsReadAsync();
        Task<Result<bool>> DeleteNotificationAsync(Guid id);
        Task CreateNotificationAsync(string userId, string title, string message, NotificationType type, Guid? referenceId = null, string? referenceType = null);
    }
}
