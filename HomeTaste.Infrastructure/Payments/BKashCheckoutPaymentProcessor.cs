using HomeTaste.Application.Interfaces.Payment;

namespace HomeTaste.Infrastructure.Payments
{
    /// <summary>
    /// bKash Checkout — redirect-based flow (grant token → createPayment → redirect → execute).
    /// Full bKash Checkout API integration is not yet implemented.
    /// </summary>
    public class BKashCheckoutPaymentProcessor : IPaymentProcessor
    {
        public string Slug => "bkash-checkout";

        public Task<PaymentInitiateResult> InitiateAsync(
            Dictionary<string, string> config,
            decimal amount,
            Guid orderId,
            Guid transactionId,
            string successUrl,
            string cancelUrl)
        {
            // TODO: call bKash grant token → createPayment → return bKash redirect URL
            return Task.FromResult(new PaymentInitiateResult
            {
                Success = false,
                Error = "bKash Checkout is not yet available. Please use bKash manual payment.",
            });
        }

        public Task<PaymentVerifyResult> VerifyAsync(
            Dictionary<string, string> config,
            string? storedRef,
            string? requestRef)
        {
            // TODO: call bKash executePayment with paymentID from callback
            return Task.FromResult(new PaymentVerifyResult
            {
                Success = false,
                Error = "bKash Checkout verification is not yet implemented.",
            });
        }
    }
}
