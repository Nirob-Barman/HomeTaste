using HomeTaste.Application.Features.Orders;

namespace HomeTaste.Application.Interfaces.Order
{
    public interface IPdfInvoiceService
    {
        byte[] Generate(OrderResponse order);
    }
}
