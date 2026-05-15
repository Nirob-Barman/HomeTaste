using HomeTaste.Application.DTOs.Delivery;
using HomeTaste.Application.Wrappers;

namespace HomeTaste.Application.Interfaces.Delivery
{
    public interface IDeliveryZoneService
    {
        Task<Result<IEnumerable<DeliveryZoneResponse>>> GetAllAsync();
        Task<Result<DeliveryZoneResponse>> GetByIdAsync(Guid id);
        Task<Result<DeliveryZoneResponse>> CreateAsync(CreateDeliveryZoneRequest request);
        Task<Result<DeliveryZoneResponse>> UpdateAsync(Guid id, UpdateDeliveryZoneRequest request);
        Task<Result<bool>> DeleteAsync(Guid id);
        Task<Result<ServiceabilityResponse>> CheckServiceabilityAsync(Guid addressId);
    }
}
