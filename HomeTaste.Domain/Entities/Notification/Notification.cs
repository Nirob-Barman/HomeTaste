using HomeTaste.Domain.Enums;

namespace HomeTaste.Domain.Entities.Notification
{
    public class Notification : BaseEntity
    {
        public string? UserId { get; private set; }
        public string? Title { get; private set; }
        public string? Message { get; private set; }
        public NotificationType Type { get; private set; }
        public bool IsRead { get; private set; }
        public Guid? ReferenceId { get; private set; }
        public string? ReferenceType { get; private set; }

        private Notification() { } // EF Core

        public static Notification Create(string userId, string title, string message, NotificationType type, Guid? referenceId, string? referenceType)
        {
            return new Notification
            {
                UserId = userId,
                Title = title,
                Message = message,
                Type = type,
                IsRead = false,
                ReferenceId = referenceId,
                ReferenceType = referenceType,
                CreatedAt = DateTime.UtcNow
            };
        }

        public void MarkAsRead()
        {
            IsRead = true;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
