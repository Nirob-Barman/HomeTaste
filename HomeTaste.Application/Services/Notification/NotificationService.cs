using HomeTaste.Application.DTOs.Notification;
using HomeTaste.Application.Helpers.Pagination;
using HomeTaste.Application.Interfaces;
using HomeTaste.Application.Interfaces.Notification;
using HomeTaste.Application.Interfaces.Persistence;
using HomeTaste.Application.Interfaces.Realtime;
using HomeTaste.Application.Wrappers;
using HomeTaste.Domain.Enums;

namespace HomeTaste.Application.Services.Notification
{
    public class NotificationService : INotificationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserContextService _userContextService;
        private readonly IRealtimeNotificationService _realtimeService;

        public NotificationService(
            IUnitOfWork unitOfWork,
            IUserContextService userContextService,
            IRealtimeNotificationService realtimeService)
        {
            _unitOfWork = unitOfWork;
            _userContextService = userContextService;
            _realtimeService = realtimeService;
        }

        public async Task<Result<PaginatedResponse<IEnumerable<NotificationResponse>>>> GetMyNotificationsAsync(int pageNumber = 1, int pageSize = 20)
        {
            var userId = _userContextService.UserId;
            if (string.IsNullOrEmpty(userId))
                return Result<PaginatedResponse<IEnumerable<NotificationResponse>>>.Fail("Invalid user.", "Unauthorized", ResultType.Unauthorized);

            var query = _unitOfWork.Repository<Domain.Entities.Notification.Notification>()
                .GetAllAsQueryable()
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt);

            var totalCount = await _unitOfWork.Repository<Domain.Entities.Notification.Notification>().CountAsync(query);

            var paged = _unitOfWork.Repository<Domain.Entities.Notification.Notification>().PaginateAsQueryable(query, pageNumber, pageSize);
            var notifications = await _unitOfWork.Repository<Domain.Entities.Notification.Notification>().ToEnumerableAsync(paged, n => MapToResponse(n));

            var meta = PaginationHelper.GetPaginationMetadata(pageNumber, pageSize, totalCount);
            return Result<PaginatedResponse<IEnumerable<NotificationResponse>>>.Ok(
                new PaginatedResponse<IEnumerable<NotificationResponse>> { Data = notifications, MetaData = meta },
                "Notifications retrieved successfully.", ResultType.Success);
        }

        public async Task<Result<UnreadCountResponse>> GetUnreadCountAsync()
        {
            var userId = _userContextService.UserId;
            if (string.IsNullOrEmpty(userId))
                return Result<UnreadCountResponse>.Fail("Invalid user.", "Unauthorized", ResultType.Unauthorized);

            var count = await _unitOfWork.Repository<Domain.Entities.Notification.Notification>()
                .CountAsync(n => n.UserId == userId && !n.IsRead);

            return Result<UnreadCountResponse>.Ok(new UnreadCountResponse { Count = count }, "Unread count retrieved.", ResultType.Success);
        }

        public async Task<Result<bool>> MarkAsReadAsync(Guid id)
        {
            var userId = _userContextService.UserId;
            var notification = await _unitOfWork.Repository<Domain.Entities.Notification.Notification>().GetByIdAsync(id);

            if (notification == null)
                return Result<bool>.Fail("Notification not found.", "Not found", ResultType.NotFound);

            if (notification.UserId != userId && !_userContextService.IsInRole("Admin"))
                return Result<bool>.Fail("Access denied.", "Forbidden", ResultType.Forbidden);

            notification.IsRead = true;
            notification.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.Repository<Domain.Entities.Notification.Notification>().Update(notification);
            await _unitOfWork.SaveChangesAsync();

            return Result<bool>.Ok(true, "Notification marked as read.", ResultType.Success);
        }

        public async Task<Result<bool>> MarkAllAsReadAsync()
        {
            var userId = _userContextService.UserId;
            if (string.IsNullOrEmpty(userId))
                return Result<bool>.Fail("Invalid user.", "Unauthorized", ResultType.Unauthorized);

            var unread = await _unitOfWork.Repository<Domain.Entities.Notification.Notification>()
                .Where(n => n.UserId == userId && !n.IsRead);

            foreach (var n in unread)
            {
                n.IsRead = true;
                n.UpdatedAt = DateTime.UtcNow;
                _unitOfWork.Repository<Domain.Entities.Notification.Notification>().Update(n);
            }

            await _unitOfWork.SaveChangesAsync();
            return Result<bool>.Ok(true, "All notifications marked as read.", ResultType.Success);
        }

        public async Task<Result<bool>> DeleteNotificationAsync(Guid id)
        {
            var userId = _userContextService.UserId;
            var notification = await _unitOfWork.Repository<Domain.Entities.Notification.Notification>().GetByIdAsync(id);

            if (notification == null)
                return Result<bool>.Fail("Notification not found.", "Not found", ResultType.NotFound);

            if (notification.UserId != userId && !_userContextService.IsInRole("Admin"))
                return Result<bool>.Fail("Access denied.", "Forbidden", ResultType.Forbidden);

            _unitOfWork.Repository<Domain.Entities.Notification.Notification>().Remove(notification);
            await _unitOfWork.SaveChangesAsync();

            return Result<bool>.Ok(true, "Notification deleted.", ResultType.Success);
        }

        public async Task CreateNotificationAsync(
            string userId,
            string title,
            string message,
            NotificationType type,
            Guid? referenceId = null,
            string? referenceType = null)
        {
            var notification = new Domain.Entities.Notification.Notification
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Title = title,
                Message = message,
                Type = type,
                IsRead = false,
                ReferenceId = referenceId,
                ReferenceType = referenceType,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Repository<Domain.Entities.Notification.Notification>().AddAsync(notification);
            await _unitOfWork.SaveChangesAsync();

            // Push real-time notification — fire-and-forget; failures are non-critical
            try
            {
                await _realtimeService.SendNotificationAsync(userId, MapToResponse(notification));
            }
            catch
            {
                // Real-time push is best-effort; DB record is already persisted
            }
        }

        private static NotificationResponse MapToResponse(Domain.Entities.Notification.Notification n) => new()
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
