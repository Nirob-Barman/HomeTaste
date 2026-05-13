using HomeTaste.Application.DTOs.Address;
using HomeTaste.Application.Wrappers;

namespace HomeTaste.Application.Interfaces.Address
{
    public interface IAddressService
    {
        Task<Result<IEnumerable<AddressResponse>>> GetMyAddressesAsync();
        Task<Result<AddressResponse>> GetAddressByIdAsync(Guid id);
        Task<Result<AddressResponse>> CreateAddressAsync(AddressRequest request);
        Task<Result<AddressResponse>> UpdateAddressAsync(Guid id, AddressRequest request);
        Task<Result<bool>> DeleteAddressAsync(Guid id);
        Task<Result<bool>> SetDefaultAddressAsync(Guid id);
    }
}
