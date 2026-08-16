using HomeTaste.Domain.Enums;

namespace HomeTaste.Application.Interfaces.Notification
{
    // Kept as a cross-cutting Application service (not converted to a Command) — invoked as a
    // side-effect from Order/Payment, not from a controller. See plan.md's Notification entry.
    public interface INotificationService
    {
        Task CreateNotificationAsync(string userId, string title, string message, NotificationType type, Guid? referenceId = null, string? referenceType = null);
    }
}
