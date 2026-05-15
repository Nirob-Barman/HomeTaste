using HomeTaste.Application.DTOs.Order;

namespace HomeTaste.Application.Interfaces.Order
{
    public interface IPdfInvoiceService
    {
        byte[] Generate(OrderResponse order);
    }
}
