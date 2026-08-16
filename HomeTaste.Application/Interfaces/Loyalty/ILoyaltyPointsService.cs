namespace HomeTaste.Application.Interfaces.Loyalty
{
    // Kept as a cross-cutting Application service (not converted to a Command) — invoked as a
    // side-effect from Order/Payment, not from a controller. See plan.md's Loyalty entry.
    public interface ILoyaltyPointsService
    {
        Task EarnPointsAsync(string userId, Guid orderId, decimal orderTotal);
    }
}
