using HomeTaste.Application.DTOs.Address;
using HomeTaste.Application.Interfaces;
using HomeTaste.Application.Interfaces.Address;
using HomeTaste.Application.Interfaces.Persistence;
using HomeTaste.Application.Validators.Address;
using HomeTaste.Application.Wrappers;
using HomeTaste.Domain.Entities.Address;

namespace HomeTaste.Application.Services.Address
{
    public class AddressService : IAddressService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserContextService _userContextService;

        public AddressService(IUnitOfWork unitOfWork, IUserContextService userContextService)
        {
            _unitOfWork = unitOfWork;
            _userContextService = userContextService;
        }

        public async Task<Result<IEnumerable<AddressResponse>>> GetMyAddressesAsync()
        {
            if (!Guid.TryParse(_userContextService.UserId, out var userId))
                return Result<IEnumerable<AddressResponse>>.Fail("Invalid user.", "Unauthorized", ResultType.Unauthorized);

            var addresses = await _unitOfWork.Repository<Domain.Entities.Address.Address>()
                .Where(a => a.UserId == userId);

            var response = addresses.Select(MapToResponse);
            return Result<IEnumerable<AddressResponse>>.Ok(response, "Addresses retrieved successfully", ResultType.Success);
        }

        public async Task<Result<AddressResponse>> GetAddressByIdAsync(Guid id)
        {
            if (!Guid.TryParse(_userContextService.UserId, out var userId))
                return Result<AddressResponse>.Fail("Invalid user.", "Unauthorized", ResultType.Unauthorized);

            var address = await _unitOfWork.Repository<Domain.Entities.Address.Address>().GetByIdAsync(id);
            if (address == null)
                return Result<AddressResponse>.Fail("Address not found.", "Not found", ResultType.NotFound);

            if (address.UserId != userId && !_userContextService.IsInRole("Admin"))
                return Result<AddressResponse>.Fail("Access denied.", "Forbidden", ResultType.Forbidden);

            return Result<AddressResponse>.Ok(MapToResponse(address), "Address retrieved successfully", ResultType.Success);
        }

        public async Task<Result<AddressResponse>> CreateAddressAsync(AddressRequest request)
        {
            var errors = AddressRequestValidator.Validate(request);
            if (errors.Count > 0)
                return Result<AddressResponse>.Fail(string.Join(" ", errors), "Validation failed", ResultType.ValidationError);

            if (!Guid.TryParse(_userContextService.UserId, out var userId))
                return Result<AddressResponse>.Fail("Invalid user.", "Unauthorized", ResultType.Unauthorized);

            if (request.IsDefault)
                await ClearDefaultFlagAsync(userId);

            var address = new Domain.Entities.Address.Address
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Label = request.Label,
                AddressLine1 = request.AddressLine1,
                AddressLine2 = request.AddressLine2,
                City = request.City,
                State = request.State,
                PostalCode = request.PostalCode,
                Country = request.Country,
                Latitude = request.Latitude,
                Longitude = request.Longitude,
                IsDefault = request.IsDefault,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Repository<Domain.Entities.Address.Address>().AddAsync(address);
            await _unitOfWork.SaveChangesAsync();

            return Result<AddressResponse>.Ok(MapToResponse(address), "Address created successfully", ResultType.Created);
        }

        public async Task<Result<AddressResponse>> UpdateAddressAsync(Guid id, AddressRequest request)
        {
            var errors = AddressRequestValidator.Validate(request);
            if (errors.Count > 0)
                return Result<AddressResponse>.Fail(string.Join(" ", errors), "Validation failed", ResultType.ValidationError);

            if (!Guid.TryParse(_userContextService.UserId, out var userId))
                return Result<AddressResponse>.Fail("Invalid user.", "Unauthorized", ResultType.Unauthorized);

            var address = await _unitOfWork.Repository<Domain.Entities.Address.Address>().GetByIdAsync(id);
            if (address == null)
                return Result<AddressResponse>.Fail("Address not found.", "Not found", ResultType.NotFound);

            if (address.UserId != userId)
                return Result<AddressResponse>.Fail("Access denied.", "Forbidden", ResultType.Forbidden);

            if (request.IsDefault && !address.IsDefault)
                await ClearDefaultFlagAsync(userId);

            address.Label = request.Label;
            address.AddressLine1 = request.AddressLine1;
            address.AddressLine2 = request.AddressLine2;
            address.City = request.City;
            address.State = request.State;
            address.PostalCode = request.PostalCode;
            address.Country = request.Country;
            address.Latitude = request.Latitude;
            address.Longitude = request.Longitude;
            address.IsDefault = request.IsDefault;
            address.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Repository<Domain.Entities.Address.Address>().Update(address);
            await _unitOfWork.SaveChangesAsync();

            return Result<AddressResponse>.Ok(MapToResponse(address), "Address updated successfully", ResultType.Success);
        }

        public async Task<Result<bool>> DeleteAddressAsync(Guid id)
        {
            if (!Guid.TryParse(_userContextService.UserId, out var userId))
                return Result<bool>.Fail("Invalid user.", "Unauthorized", ResultType.Unauthorized);

            var address = await _unitOfWork.Repository<Domain.Entities.Address.Address>().GetByIdAsync(id);
            if (address == null)
                return Result<bool>.Fail("Address not found.", "Not found", ResultType.NotFound);

            if (address.UserId != userId && !_userContextService.IsInRole("Admin"))
                return Result<bool>.Fail("Access denied.", "Forbidden", ResultType.Forbidden);

            _unitOfWork.Repository<Domain.Entities.Address.Address>().Remove(address);
            await _unitOfWork.SaveChangesAsync();

            return Result<bool>.Ok(true, "Address deleted successfully", ResultType.Success);
        }

        public async Task<Result<bool>> SetDefaultAddressAsync(Guid id)
        {
            if (!Guid.TryParse(_userContextService.UserId, out var userId))
                return Result<bool>.Fail("Invalid user.", "Unauthorized", ResultType.Unauthorized);

            var address = await _unitOfWork.Repository<Domain.Entities.Address.Address>().GetByIdAsync(id);
            if (address == null)
                return Result<bool>.Fail("Address not found.", "Not found", ResultType.NotFound);

            if (address.UserId != userId)
                return Result<bool>.Fail("Access denied.", "Forbidden", ResultType.Forbidden);

            await ClearDefaultFlagAsync(userId);

            address.IsDefault = true;
            address.UpdatedAt = DateTime.UtcNow;

            _unitOfWork.Repository<Domain.Entities.Address.Address>().Update(address);
            await _unitOfWork.SaveChangesAsync();

            return Result<bool>.Ok(true, "Default address updated successfully", ResultType.Success);
        }

        private async Task ClearDefaultFlagAsync(Guid userId)
        {
            var existingDefaults = await _unitOfWork.Repository<Domain.Entities.Address.Address>()
                .Where(a => a.UserId == userId && a.IsDefault);

            foreach (var existing in existingDefaults)
            {
                existing.IsDefault = false;
                existing.UpdatedAt = DateTime.UtcNow;
                _unitOfWork.Repository<Domain.Entities.Address.Address>().Update(existing);
            }
        }

        private static AddressResponse MapToResponse(Domain.Entities.Address.Address address) => new()
        {
            Id = address.Id,
            UserId = address.UserId,
            Label = address.Label,
            AddressLine1 = address.AddressLine1,
            AddressLine2 = address.AddressLine2,
            City = address.City,
            State = address.State,
            PostalCode = address.PostalCode,
            Country = address.Country,
            Latitude = address.Latitude,
            Longitude = address.Longitude,
            IsDefault = address.IsDefault,
            CreatedAt = address.CreatedAt
        };
    }
}
