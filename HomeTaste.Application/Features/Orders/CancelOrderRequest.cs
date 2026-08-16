namespace HomeTaste.Application.Features.Orders
{
    public record CancelOrderRequest
    {
        public string? Reason { get; set; }
    }
}
