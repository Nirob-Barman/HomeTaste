namespace HomeTaste.Application.Interfaces.Order
{
    public interface IDeliveryFeeService
    {
        /// <summary>
        /// Returns the delivery fee for a given subtotal.
        /// Free when subtotal >= $50, $2.99 when >= $25, otherwise $4.99.
        /// </summary>
        decimal Calculate(decimal subTotal);
    }
}
