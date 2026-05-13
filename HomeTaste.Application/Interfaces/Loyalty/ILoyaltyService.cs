using HomeTaste.Application.DTOs.Loyalty;
using HomeTaste.Application.Wrappers;

namespace HomeTaste.Application.Interfaces.Loyalty
{
    public interface ILoyaltyService
    {
        Task<Result<LoyaltyAccountResponse>> GetMyAccountAsync();
        Task<Result<PaginatedResponse<IEnumerable<LoyaltyTransactionResponse>>>> GetMyTransactionsAsync(int pageNumber = 1, int pageSize = 20);
        Task<Result<PointsPreviewResponse>> PreviewRedemptionAsync(int pointsToRedeem);
        Task<Result<LoyaltyAccountResponse>> GetAccountByUserIdAsync(string userId);
        Task<Result<bool>> AdjustPointsAsync(AdjustPointsRequest request);
        Task EarnPointsAsync(string userId, Guid orderId, decimal orderTotal);
    }
}
