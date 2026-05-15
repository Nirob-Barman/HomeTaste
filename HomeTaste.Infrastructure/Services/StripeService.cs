using HomeTaste.Application.Interfaces.Payment;
using Stripe;

namespace HomeTaste.Infrastructure.Services
{
    public class StripeService : IStripeService
    {
        public async Task<(string PaymentIntentId, string ClientSecret)> CreatePaymentIntentAsync(
            string secretKey, decimal amount, Guid orderId, Guid transactionId)
        {
            var client = new StripeClient(secretKey);
            var options = new PaymentIntentCreateOptions
            {
                Amount = (long)(amount * 100),
                Currency = "usd",
                PaymentMethodTypes = ["card"],
                Metadata = new Dictionary<string, string>
                {
                    { "orderId", orderId.ToString() },
                    { "transactionId", transactionId.ToString() }
                }
            };

            var service = new PaymentIntentService(client);
            var intent = await service.CreateAsync(options);
            return (intent.Id, intent.ClientSecret);
        }

        public async Task<bool> VerifyPaymentIntentAsync(string secretKey, string paymentIntentId)
        {
            var client = new StripeClient(secretKey);
            var service = new PaymentIntentService(client);
            var intent = await service.GetAsync(paymentIntentId);
            return intent.Status == "succeeded";
        }
    }
}
