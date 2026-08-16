namespace HomeTaste.Application.Features.Notifications
{
    public static class NotificationMapper
    {
        public static NotificationResponse ToResponse(HomeTaste.Domain.Entities.Notification.Notification n) => new()
        {
            Id = n.Id,
            UserId = n.UserId,
            Title = n.Title,
            Message = n.Message,
            Type = n.Type,
            TypeLabel = n.Type.ToString(),
            IsRead = n.IsRead,
            ReferenceId = n.ReferenceId,
            ReferenceType = n.ReferenceType,
            CreatedAt = n.CreatedAt
        };
    }
}
