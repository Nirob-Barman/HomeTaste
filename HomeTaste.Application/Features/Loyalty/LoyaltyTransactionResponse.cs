using HomeTaste.Domain.Enums;

namespace HomeTaste.Application.Features.Loyalty
{
    public record LoyaltyTransactionResponse(
        Guid Id,
        int Points,
        LoyaltyTransactionType TransactionType,
        string TypeLabel,
        Guid? ReferenceId,
        string? Description,
        DateTime? CreatedAt);
}
