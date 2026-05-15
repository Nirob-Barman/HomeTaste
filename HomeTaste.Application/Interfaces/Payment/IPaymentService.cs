using HomeTaste.Application.DTOs.Payment;
using HomeTaste.Application.Wrappers;
using HomeTaste.Domain.Enums;

namespace HomeTaste.Application.Interfaces.Payment
{
    public interface IPaymentService
    {
        Task<Result<PaymentTransactionResponse>> InitiatePaymentAsync(InitiatePaymentRequest request, string callbackBaseUrl);
        Task<Result<PaymentTransactionResponse>> ConfirmPaymentAsync(Guid transactionId, ConfirmPaymentRequest request);
        Task<Result<PaymentTransactionResponse>> RefundPaymentAsync(Guid transactionId, RefundPaymentRequest request);
        Task<Result<PaymentTransactionResponse>> GetPaymentByOrderIdAsync(Guid orderId);
        Task<Result<PaymentTransactionResponse>> GetPaymentByIdAsync(Guid id);
        Task<Result<PaginatedResponse<IEnumerable<PaymentTransactionResponse>>>> GetAllPaymentsAsync(int pageNumber = 1, int pageSize = 10, PaymentStatus? status = null);
    }
}
