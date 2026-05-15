using HomeTaste.Application.Interfaces.Order;

namespace HomeTaste.Application.Services.Order
{
    public class DeliveryFeeService : IDeliveryFeeService
    {
        private const decimal FreeThreshold = 50m;
        private const decimal MidThreshold = 25m;
        private const decimal MidFee = 2.99m;
        private const decimal BaseFee = 4.99m;

        public decimal Calculate(decimal subTotal)
        {
            if (subTotal >= FreeThreshold) return 0m;
            if (subTotal >= MidThreshold) return MidFee;
            return BaseFee;
        }
    }
}
