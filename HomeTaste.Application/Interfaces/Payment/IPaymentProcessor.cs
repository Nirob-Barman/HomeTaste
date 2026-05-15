namespace HomeTaste.Application.Interfaces.Payment
{
    public interface IPaymentProcessor
    {
        /// <summary>Matches PaymentGateway.Slug (unique per provider, e.g. "stripe", "bkash", "bkash-checkout")</summary>
        string Slug { get; }

        Task<PaymentInitiateResult> InitiateAsync(
            Dictionary<string, string> config,
            decimal amount,
            Guid orderId,
            Guid transactionId,
            string successUrl,
            string cancelUrl);

        Task<PaymentVerifyResult> VerifyAsync(
            Dictionary<string, string> config,
            string? storedRef,    // ref stored during initiate (e.g. Stripe PaymentIntentId)
            string? requestRef);  // ref supplied by caller (e.g. bKash TXN ID from customer)
    }

    public class PaymentInitiateResult
    {
        public bool Success { get; set; }
        public string? Error { get; set; }
        public string? ProviderRef { get; set; }    // to store as TransactionRef
        public string? ClientSecret { get; set; }   // Stripe Elements
        public string? PublishableKey { get; set; } // Stripe Elements
        public string? MerchantNumber { get; set; } // bKash Manual: number customer sends to
        public string? RedirectUrl { get; set; }    // bKash Checkout: redirect URL
    }

    public class PaymentVerifyResult
    {
        public bool Success { get; set; }
        public string? TransactionRef { get; set; } // ref to persist on the transaction
        public string? Error { get; set; }
    }
}
