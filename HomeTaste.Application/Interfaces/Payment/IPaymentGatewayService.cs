using HomeTaste.Application.DTOs.Payment;
using HomeTaste.Application.Wrappers;

namespace HomeTaste.Application.Interfaces.Payment
{
    public interface IPaymentGatewayService
    {
        Task<Result<List<PaymentGatewayResponse>>> GetAllAsync();
        Task<Result<List<PaymentGatewayResponse>>> GetActiveAsync();
        Task<Result<PaymentGatewayResponse>> GetByIdAsync(Guid id);
        Task<Result<PaymentGatewayResponse>> CreateAsync(CreatePaymentGatewayRequest request);
        Task<Result<PaymentGatewayResponse>> UpdateAsync(Guid id, UpdatePaymentGatewayRequest request);
        Task<Result<PaymentGatewayResponse>> ToggleActiveAsync(Guid id);
        Task<Result<bool>> DeleteAsync(Guid id);
    }
}
