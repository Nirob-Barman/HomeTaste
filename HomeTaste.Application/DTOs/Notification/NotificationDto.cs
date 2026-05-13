using HomeTaste.Domain.Enums;

namespace HomeTaste.Application.DTOs.Notification
{
    public class NotificationResponse
    {
        public Guid Id { get; set; }
        public string? UserId { get; set; }
        public string? Title { get; set; }
        public string? Message { get; set; }
        public NotificationType Type { get; set; }
        public string? TypeLabel { get; set; }
        public bool IsRead { get; set; }
        public Guid? ReferenceId { get; set; }
        public string? ReferenceType { get; set; }
        public DateTime? CreatedAt { get; set; }
    }

    public class UnreadCountResponse
    {
        public int Count { get; set; }
    }
}
