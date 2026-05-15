using HomeTaste.Application.Interfaces.Payment;

namespace HomeTaste.Infrastructure.Payments
{
    public class PaymentProcessorFactory : IPaymentProcessorFactory
    {
        private readonly IEnumerable<IPaymentProcessor> _processors;

        public PaymentProcessorFactory(IEnumerable<IPaymentProcessor> processors)
            => _processors = processors;

        public IPaymentProcessor? GetProcessor(string slug)
            => _processors.FirstOrDefault(p => p.Slug == slug);
    }
}
