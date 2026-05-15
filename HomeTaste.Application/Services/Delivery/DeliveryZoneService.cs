using HomeTaste.Application.DTOs.Delivery;
using HomeTaste.Application.Interfaces.Delivery;
using HomeTaste.Application.Interfaces.Persistence;
using HomeTaste.Application.Wrappers;
using HomeTaste.Domain.Entities.Delivery;

namespace HomeTaste.Application.Services.Delivery
{
    public class DeliveryZoneService : IDeliveryZoneService
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeliveryZoneService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<IEnumerable<DeliveryZoneResponse>>> GetAllAsync()
        {
            var zones = await _unitOfWork.Repository<DeliveryZone>().GetAllAsync();
            return Result<IEnumerable<DeliveryZoneResponse>>.Ok(
                zones.Select(ToResponse),
                "Zones retrieved.", ResultType.Success);
        }

        public async Task<Result<DeliveryZoneResponse>> GetByIdAsync(Guid id)
        {
            var zone = await _unitOfWork.Repository<DeliveryZone>().GetByIdAsync(id);
            if (zone == null)
                return Result<DeliveryZoneResponse>.Fail("Zone not found.", "Not found", ResultType.NotFound);
            return Result<DeliveryZoneResponse>.Ok(ToResponse(zone), "Zone retrieved.", ResultType.Success);
        }

        public async Task<Result<DeliveryZoneResponse>> CreateAsync(CreateDeliveryZoneRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
                return Result<DeliveryZoneResponse>.Fail("Name is required.", "Validation failed", ResultType.ValidationError);

            var zone = new DeliveryZone
            {
                Id = Guid.NewGuid(),
                Name = request.Name.Trim(),
                Description = request.Description?.Trim(),
                IsActive = request.IsActive,
                AllowedCities = request.AllowedCities.Select(c => c.Trim().ToLowerInvariant()).ToList(),
                AllowedPostalCodes = request.AllowedPostalCodes.Select(p => p.Trim().ToLowerInvariant()).ToList(),
            };

            await _unitOfWork.Repository<DeliveryZone>().AddAsync(zone);
            await _unitOfWork.SaveChangesAsync();
            return Result<DeliveryZoneResponse>.Ok(ToResponse(zone), "Zone created.", ResultType.Created);
        }

        public async Task<Result<DeliveryZoneResponse>> UpdateAsync(Guid id, UpdateDeliveryZoneRequest request)
        {
            var zone = await _unitOfWork.Repository<DeliveryZone>().GetByIdAsync(id);
            if (zone == null)
                return Result<DeliveryZoneResponse>.Fail("Zone not found.", "Not found", ResultType.NotFound);

            if (string.IsNullOrWhiteSpace(request.Name))
                return Result<DeliveryZoneResponse>.Fail("Name is required.", "Validation failed", ResultType.ValidationError);

            zone.Name = request.Name.Trim();
            zone.Description = request.Description?.Trim();
            zone.IsActive = request.IsActive;
            zone.AllowedCities = request.AllowedCities.Select(c => c.Trim().ToLowerInvariant()).ToList();
            zone.AllowedPostalCodes = request.AllowedPostalCodes.Select(p => p.Trim().ToLowerInvariant()).ToList();

            _unitOfWork.Repository<DeliveryZone>().Update(zone);
            await _unitOfWork.SaveChangesAsync();
            return Result<DeliveryZoneResponse>.Ok(ToResponse(zone), "Zone updated.", ResultType.Success);
        }

        public async Task<Result<bool>> DeleteAsync(Guid id)
        {
            var zone = await _unitOfWork.Repository<DeliveryZone>().GetByIdAsync(id);
            if (zone == null)
                return Result<bool>.Fail("Zone not found.", "Not found", ResultType.NotFound);

            _unitOfWork.Repository<DeliveryZone>().Remove(zone);
            await _unitOfWork.SaveChangesAsync();
            return Result<bool>.Ok(true, "Zone deleted.", ResultType.Success);
        }

        public async Task<Result<ServiceabilityResponse>> CheckServiceabilityAsync(Guid addressId)
        {
            var address = await _unitOfWork.Repository<Domain.Entities.Address.Address>().GetByIdAsync(addressId);
            if (address == null)
                return Result<ServiceabilityResponse>.Fail("Address not found.", "Not found", ResultType.NotFound);

            var activeZones = await _unitOfWork.Repository<DeliveryZone>()
                .Where(z => z.IsActive);

            // No zones configured — treat as fully open (graceful degradation)
            if (!activeZones.Any())
                return Result<ServiceabilityResponse>.Ok(new ServiceabilityResponse
                {
                    IsServiceable = true,
                    Message = "All areas are currently served."
                }, "Serviceability checked.", ResultType.Success);

            var city = address.City?.Trim().ToLowerInvariant() ?? "";
            var postal = address.PostalCode?.Trim().ToLowerInvariant() ?? "";

            var matchedZone = activeZones.FirstOrDefault(z =>
                (!string.IsNullOrEmpty(city) && z.AllowedCities.Contains(city)) ||
                (!string.IsNullOrEmpty(postal) && z.AllowedPostalCodes.Contains(postal)));

            if (matchedZone != null)
                return Result<ServiceabilityResponse>.Ok(new ServiceabilityResponse
                {
                    IsServiceable = true,
                    ZoneName = matchedZone.Name,
                    Message = $"Delivery available via {matchedZone.Name}."
                }, "Serviceability checked.", ResultType.Success);

            return Result<ServiceabilityResponse>.Ok(new ServiceabilityResponse
            {
                IsServiceable = false,
                Message = "Sorry, we don't deliver to this address yet."
            }, "Serviceability checked.", ResultType.Success);
        }

        private static DeliveryZoneResponse ToResponse(DeliveryZone zone) => new()
        {
            Id = zone.Id,
            Name = zone.Name,
            Description = zone.Description,
            IsActive = zone.IsActive,
            AllowedCities = zone.AllowedCities,
            AllowedPostalCodes = zone.AllowedPostalCodes,
        };
    }
}
