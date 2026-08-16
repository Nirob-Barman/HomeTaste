using HomeTaste.Application.Common.Exceptions;
using HomeTaste.Application.Interfaces.Persistence;
using HomeTaste.Application.Wrappers;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HomeTaste.Application.Features.DeliveryZones.Queries.CheckServiceability
{
    public class CheckServiceabilityQueryHandler : IRequestHandler<CheckServiceabilityQuery, Result<ServiceabilityResponse>>
    {
        private readonly IApplicationDbContext _context;

        public CheckServiceabilityQueryHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result<ServiceabilityResponse>> Handle(CheckServiceabilityQuery request, CancellationToken cancellationToken)
        {
            var address = await _context.Addresses.FindAsync(new object?[] { request.AddressId }, cancellationToken);
            if (address == null)
                throw new NotFoundException("Address not found.");

            var activeZones = await _context.DeliveryZones.Where(z => z.IsActive).ToListAsync(cancellationToken);

            // No zones configured — treat as fully open (graceful degradation)
            if (!activeZones.Any())
                return Result<ServiceabilityResponse>.Ok(new ServiceabilityResponse
                {
                    IsServiceable = true,
                    Message = "All areas are currently served."
                }, "Serviceability checked.");

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
                }, "Serviceability checked.");

            return Result<ServiceabilityResponse>.Ok(new ServiceabilityResponse
            {
                IsServiceable = false,
                Message = "Sorry, we don't deliver to this address yet."
            }, "Serviceability checked.");
        }
    }
}
