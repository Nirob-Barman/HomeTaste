using HomeTaste.Domain.Enums;

namespace HomeTaste.Domain.Entities.Notification
{
    public class Notification : BaseEntity
    {
        public Guid Id { get; set; }
        public string? UserId { get; set; }
        public string? Title { get; set; }
        public string? Message { get; set; }
        public NotificationType Type { get; set; }
        public bool IsRead { get; set; }
        public Guid? ReferenceId { get; set; }
        public string? ReferenceType { get; set; }
    }
}
