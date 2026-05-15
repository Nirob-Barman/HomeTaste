using HomeTaste.Application.Interfaces.Payment;

namespace HomeTaste.Infrastructure.Payments
{
    /// <summary>
    /// bKash manual payment. Customer sends money via the bKash app and supplies
    /// their TXN ID. No outbound API call — the reference is trusted as-is.
    /// </summary>
    public class BKashManualPaymentProcessor : IPaymentProcessor
    {
        public string Slug => "bkash";

        public Task<PaymentInitiateResult> InitiateAsync(
            Dictionary<string, string> config,
            decimal amount,
            Guid orderId,
            Guid transactionId,
            string successUrl,
            string cancelUrl)
        {
            config.TryGetValue("MerchantNumber", out var merchantNumber);

            return Task.FromResult(new PaymentInitiateResult
            {
                Success = true,
                MerchantNumber = merchantNumber,
            });
        }

        public Task<PaymentVerifyResult> VerifyAsync(
            Dictionary<string, string> config,
            string? storedRef,
            string? requestRef)
        {
            if (string.IsNullOrWhiteSpace(requestRef))
                return Task.FromResult(new PaymentVerifyResult
                {
                    Success = false,
                    Error = "Transaction reference is required for bKash manual payment."
                });

            return Task.FromResult(new PaymentVerifyResult
            {
                Success = true,
                TransactionRef = requestRef.Trim(),
            });
        }
    }
}
