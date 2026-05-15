namespace HomeTaste.Application.Interfaces.Payment
{
    public interface IStripeService
    {
        Task<(string PaymentIntentId, string ClientSecret)> CreatePaymentIntentAsync(
            string secretKey, decimal amount, Guid orderId, Guid transactionId);

        Task<bool> VerifyPaymentIntentAsync(string secretKey, string paymentIntentId);
    }
}
