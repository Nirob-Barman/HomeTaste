using HomeTaste.Application.Interfaces.Payment;

namespace HomeTaste.Infrastructure.Payments
{
    /// <summary>Stripe card payments via Stripe Elements (client-side completion).</summary>
    public class StripePaymentProcessor : IPaymentProcessor
    {
        private readonly IStripeService _stripe;

        public StripePaymentProcessor(IStripeService stripe) => _stripe = stripe;

        public string Slug => "stripe";

        public async Task<PaymentInitiateResult> InitiateAsync(
            Dictionary<string, string> config,
            decimal amount,
            Guid orderId,
            Guid transactionId,
            string successUrl,
            string cancelUrl)
        {
            config.TryGetValue("SecretKey", out var secretKey);
            if (string.IsNullOrEmpty(secretKey))
                return new PaymentInitiateResult { Success = false, Error = "Stripe secret key is not configured." };

            try
            {
                var (paymentIntentId, clientSecret) = await _stripe.CreatePaymentIntentAsync(
                    secretKey, amount, orderId, transactionId);

                config.TryGetValue("PublishableKey", out var publishableKey);

                return new PaymentInitiateResult
                {
                    Success = true,
                    ProviderRef = paymentIntentId,
                    ClientSecret = clientSecret,
                    PublishableKey = publishableKey,
                };
            }
            catch
            {
                return new PaymentInitiateResult { Success = false, Error = "Failed to create Stripe payment intent." };
            }
        }

        public async Task<PaymentVerifyResult> VerifyAsync(
            Dictionary<string, string> config,
            string? storedRef,
            string? requestRef)
        {
            if (string.IsNullOrEmpty(storedRef))
                return new PaymentVerifyResult { Success = false, Error = "No payment intent reference found." };

            config.TryGetValue("SecretKey", out var secretKey);
            if (string.IsNullOrEmpty(secretKey))
                return new PaymentVerifyResult { Success = false, Error = "Stripe secret key is not configured." };

            try
            {
                var verified = await _stripe.VerifyPaymentIntentAsync(secretKey, storedRef);
                return new PaymentVerifyResult { Success = verified, TransactionRef = storedRef };
            }
            catch
            {
                return new PaymentVerifyResult { Success = false, Error = "Stripe verification failed." };
            }
        }
    }
}
