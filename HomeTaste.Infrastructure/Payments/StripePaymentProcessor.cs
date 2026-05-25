using HomeTaste.Application.Interfaces.Payment;
using Stripe;

namespace HomeTaste.Infrastructure.Payments
{
    /// <summary>Stripe card payments via Stripe Elements (client-side completion).</summary>
    public class StripePaymentProcessor : IPaymentProcessor
    {
        private readonly IStripeService _stripe;

        public StripePaymentProcessor(IStripeService stripe) => _stripe = stripe;

        public string Slug => "stripe_payment_intents";

        public async Task<PaymentInitiateResult> InitiateAsync(
            Dictionary<string, string> config,
            decimal amount,
            Guid orderId,
            Guid transactionId,
            string successUrl,
            string cancelUrl)
        {
            config.TryGetValue("secret_key", out var secretKey);
            if (string.IsNullOrEmpty(secretKey))
                return new PaymentInitiateResult { Success = false, Error = "Stripe secret key is not configured." };

            try
            {
                var (paymentIntentId, clientSecret) = await _stripe.CreatePaymentIntentAsync(
                    secretKey, amount, orderId, transactionId);

                config.TryGetValue("publishable_key", out var publishableKey);

                return new PaymentInitiateResult
                {
                    Success = true,
                    ProviderRef = paymentIntentId,
                    ClientSecret = clientSecret,
                    PublishableKey = publishableKey,
                };
            }
            catch (StripeException ex)
            {
                return new PaymentInitiateResult { Success = false, Error = $"Stripe error: {ex.StripeError?.Message ?? ex.Message}" };
            }
            catch (Exception ex)
            {
                return new PaymentInitiateResult { Success = false, Error = $"Failed to create Stripe payment intent: {ex.Message}" };
            }
        }

        public async Task<PaymentVerifyResult> VerifyAsync(
            Dictionary<string, string> config,
            string? storedRef,
            string? requestRef)
        {
            var intentId = storedRef ?? requestRef;
            if (string.IsNullOrEmpty(intentId))
                return new PaymentVerifyResult { Success = false, Error = "No payment intent reference found." };

            config.TryGetValue("secret_key", out var secretKey);
            if (string.IsNullOrEmpty(secretKey))
                return new PaymentVerifyResult { Success = false, Error = "Stripe secret key is not configured." };

            try
            {
                var verified = await _stripe.VerifyPaymentIntentAsync(secretKey, intentId);
                return new PaymentVerifyResult { Success = verified, TransactionRef = intentId };
            }
            catch (StripeException ex)
            {
                return new PaymentVerifyResult { Success = false, Error = $"Stripe error: {ex.StripeError?.Message ?? ex.Message}" };
            }
            catch (Exception ex)
            {
                return new PaymentVerifyResult { Success = false, Error = $"Stripe verification failed: {ex.Message}" };
            }
        }
    }
}
