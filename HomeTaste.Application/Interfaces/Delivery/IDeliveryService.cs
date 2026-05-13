using HomeTaste.Application.DTOs.Delivery;
using HomeTaste.Application.Wrappers;

namespace HomeTaste.Application.Interfaces.Delivery
{
    public interface IDeliveryService
    {
        Task<Result<PaginatedResponse<IEnumerable<DeliveryPersonnelResponse>>>> GetAllDeliveryPersonnelAsync(int pageNumber = 1, int pageSize = 10);
        Task<Result<DeliveryPersonnelResponse>> GetDeliveryPersonnelByIdAsync(Guid id);
        Task<Result<DeliveryPersonnelResponse>> CreateDeliveryPersonnelAsync(CreateDeliveryPersonnelRequest request);
        Task<Result<DeliveryPersonnelResponse>> UpdateDeliveryPersonnelAsync(Guid id, UpdateDeliveryPersonnelRequest request);
        Task<Result<bool>> DeleteDeliveryPersonnelAsync(Guid id);
        Task<Result<bool>> ToggleAvailabilityAsync(Guid id);
        Task<Result<bool>> UpdateLocationAsync(Guid id, UpdateLocationRequest request);
        Task<Result<DeliveryAssignmentResponse>> AssignDeliveryAsync(AssignDeliveryRequest request);
        Task<Result<DeliveryAssignmentResponse>> UpdateDeliveryStatusAsync(Guid assignmentId, UpdateDeliveryStatusRequest request);
        Task<Result<DeliveryAssignmentResponse>> GetDeliveryByOrderIdAsync(Guid orderId);
        Task<Result<IEnumerable<DeliveryAssignmentResponse>>> GetMyAssignedDeliveriesAsync();
    }
}
