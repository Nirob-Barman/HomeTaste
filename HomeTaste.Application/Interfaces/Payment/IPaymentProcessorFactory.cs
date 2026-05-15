namespace HomeTaste.Application.Interfaces.Payment
{
    public interface IPaymentProcessorFactory
    {
        IPaymentProcessor? GetProcessor(string slug);
    }
}
