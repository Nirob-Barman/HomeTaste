using HomeTaste.Application.DTOs.Loyalty;
using HomeTaste.Application.Helpers.Pagination;
using HomeTaste.Application.Validators.Loyalty;
using HomeTaste.Application.Interfaces;
using HomeTaste.Application.Interfaces.Loyalty;
using HomeTaste.Application.Interfaces.Persistence;
using HomeTaste.Application.Wrappers;
using HomeTaste.Domain.Entities.Loyalty;
using HomeTaste.Domain.Enums;

namespace HomeTaste.Application.Services.Loyalty
{
    public class LoyaltyService : ILoyaltyService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserContextService _userContextService;

        // Tier thresholds (total points earned, not current balance)
        private static readonly (int MinPoints, LoyaltyTier Tier)[] TierThresholds =
        [
            (10_000, LoyaltyTier.Platinum),
            (5_000, LoyaltyTier.Gold),
            (1_000, LoyaltyTier.Silver),
            (0,     LoyaltyTier.Bronze)
        ];

        // 100 points = $1
        private const int PointsPerDollar = 1;
        private const int PointsRedemptionRate = 100;

        public LoyaltyService(IUnitOfWork unitOfWork, IUserContextService userContextService)
        {
            _unitOfWork = unitOfWork;
            _userContextService = userContextService;
        }

        public async Task<Result<LoyaltyAccountResponse>> GetMyAccountAsync()
        {
            var userId = _userContextService.UserId;
            if (string.IsNullOrEmpty(userId))
                return Result<LoyaltyAccountResponse>.Fail("Invalid user.", "Unauthorized", ResultType.Unauthorized);

            var account = await GetOrCreateAccountAsync(userId);
            return Result<LoyaltyAccountResponse>.Ok(MapToResponse(account), "Loyalty account retrieved.", ResultType.Success);
        }

        public async Task<Result<PaginatedResponse<IEnumerable<LoyaltyTransactionResponse>>>> GetMyTransactionsAsync(int pageNumber = 1, int pageSize = 20)
        {
            var userId = _userContextService.UserId;
            if (string.IsNullOrEmpty(userId))
                return Result<PaginatedResponse<IEnumerable<LoyaltyTransactionResponse>>>.Fail("Invalid user.", "Unauthorized", ResultType.Unauthorized);

            var account = await _unitOfWork.Repository<LoyaltyAccount>()
                .FirstOrDefaultAsync(a => a.UserId == userId);

            if (account == null)
                return Result<PaginatedResponse<IEnumerable<LoyaltyTransactionResponse>>>.Ok(
                    new PaginatedResponse<IEnumerable<LoyaltyTransactionResponse>>
                    {
                        Data = Enumerable.Empty<LoyaltyTransactionResponse>(),
                        MetaData = PaginationHelper.GetPaginationMetadata(pageNumber, pageSize, 0)
                    }, "No transactions found.", ResultType.Success);

            var query = _unitOfWork.Repository<LoyaltyTransaction>()
                .GetAllAsQueryable()
                .Where(t => t.LoyaltyAccountId == account.Id)
                .OrderByDescending(t => t.CreatedAt);

            var totalCount = await _unitOfWork.Repository<LoyaltyTransaction>().CountAsync(query);
            var paged = _unitOfWork.Repository<LoyaltyTransaction>().PaginateAsQueryable(query, pageNumber, pageSize);
            var transactions = await _unitOfWork.Repository<LoyaltyTransaction>().ToEnumerableAsync(paged, t => MapTransactionToResponse(t));

            var meta = PaginationHelper.GetPaginationMetadata(pageNumber, pageSize, totalCount);
            return Result<PaginatedResponse<IEnumerable<LoyaltyTransactionResponse>>>.Ok(
                new PaginatedResponse<IEnumerable<LoyaltyTransactionResponse>> { Data = transactions, MetaData = meta },
                "Transactions retrieved.", ResultType.Success);
        }

        public async Task<Result<PointsPreviewResponse>> PreviewRedemptionAsync(int pointsToRedeem)
        {
            var userId = _userContextService.UserId;
            if (string.IsNullOrEmpty(userId))
                return Result<PointsPreviewResponse>.Fail("Invalid user.", "Unauthorized", ResultType.Unauthorized);

            if (pointsToRedeem <= 0)
                return Result<PointsPreviewResponse>.Fail("Points to redeem must be greater than zero.", "Bad request", ResultType.BadRequest);

            var account = await _unitOfWork.Repository<LoyaltyAccount>()
                .FirstOrDefaultAsync(a => a.UserId == userId);

            var available = account?.CurrentPoints ?? 0;

            if (pointsToRedeem > available)
                return Result<PointsPreviewResponse>.Fail($"Insufficient points. Available: {available}.", "Bad request", ResultType.BadRequest);

            var discount = Math.Round((decimal)pointsToRedeem / PointsRedemptionRate, 2);

            return Result<PointsPreviewResponse>.Ok(new PointsPreviewResponse
            {
                PointsToRedeem = pointsToRedeem,
                DiscountAmount = discount,
                RemainingPoints = available - pointsToRedeem
            }, "Redemption preview calculated.", ResultType.Success);
        }

        public async Task<Result<LoyaltyAccountResponse>> GetAccountByUserIdAsync(string userId)
        {
            var account = await _unitOfWork.Repository<LoyaltyAccount>()
                .FirstOrDefaultAsync(a => a.UserId == userId);

            if (account == null)
                return Result<LoyaltyAccountResponse>.Fail("No loyalty account found for this user.", "Not found", ResultType.NotFound);

            return Result<LoyaltyAccountResponse>.Ok(MapToResponse(account), "Account retrieved.", ResultType.Success);
        }

        public async Task<Result<bool>> AdjustPointsAsync(AdjustPointsRequest request)
        {
            var errors = AdjustPointsRequestValidator.Validate(request);
            if (errors.Count > 0)
                return Result<bool>.Fail(string.Join(" ", errors), "Validation failed", ResultType.ValidationError);

            var account = await GetOrCreateAccountAsync(request.UserId);

            if (request.Points < 0 && account.CurrentPoints + request.Points < 0)
                return Result<bool>.Fail("Adjustment would result in a negative balance.", "Bad request", ResultType.BadRequest);

            account.CurrentPoints += request.Points;
            if (request.Points > 0)
                account.TotalPointsEarned += request.Points;

            account.Tier = CalculateTier(account.TotalPointsEarned);
            account.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.Repository<LoyaltyAccount>().Update(account);

            var transaction = new LoyaltyTransaction
            {
                Id = Guid.NewGuid(),
                LoyaltyAccountId = account.Id,
                Points = request.Points,
                TransactionType = LoyaltyTransactionType.Adjusted,
                Description = request.Description ?? "Admin adjustment",
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Repository<LoyaltyTransaction>().AddAsync(transaction);
            await _unitOfWork.SaveChangesAsync();

            return Result<bool>.Ok(true, $"Points adjusted by {request.Points:+#;-#;0}.", ResultType.Success);
        }

        public async Task EarnPointsAsync(string userId, Guid orderId, decimal orderTotal)
        {
            var pointsEarned = (int)Math.Floor(orderTotal) * PointsPerDollar;
            if (pointsEarned <= 0) return;

            var account = await GetOrCreateAccountAsync(userId);

            account.CurrentPoints += pointsEarned;
            account.TotalPointsEarned += pointsEarned;
            account.Tier = CalculateTier(account.TotalPointsEarned);
            account.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.Repository<LoyaltyAccount>().Update(account);

            var transaction = new LoyaltyTransaction
            {
                Id = Guid.NewGuid(),
                LoyaltyAccountId = account.Id,
                Points = pointsEarned,
                TransactionType = LoyaltyTransactionType.Earned,
                ReferenceId = orderId,
                Description = $"Earned {pointsEarned} point{(pointsEarned == 1 ? "" : "s")} for order payment.",
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Repository<LoyaltyTransaction>().AddAsync(transaction);
            await _unitOfWork.SaveChangesAsync();
        }

        private async Task<LoyaltyAccount> GetOrCreateAccountAsync(string userId)
        {
            var account = await _unitOfWork.Repository<LoyaltyAccount>()
                .FirstOrDefaultAsync(a => a.UserId == userId);

            if (account != null) return account;

            account = new LoyaltyAccount
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                CurrentPoints = 0,
                TotalPointsEarned = 0,
                Tier = LoyaltyTier.Bronze,
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Repository<LoyaltyAccount>().AddAsync(account);
            await _unitOfWork.SaveChangesAsync();

            return account;
        }

        private static LoyaltyTier CalculateTier(int totalPointsEarned)
        {
            foreach (var (min, tier) in TierThresholds)
                if (totalPointsEarned >= min) return tier;
            return LoyaltyTier.Bronze;
        }

        private static int PointsToNextTier(int totalPointsEarned)
        {
            int[] thresholds = [1_000, 5_000, 10_000];
            foreach (var t in thresholds)
                if (totalPointsEarned < t) return t - totalPointsEarned;
            return 0;
        }

        private static LoyaltyAccountResponse MapToResponse(LoyaltyAccount a) => new()
        {
            Id = a.Id,
            UserId = a.UserId,
            CurrentPoints = a.CurrentPoints,
            TotalPointsEarned = a.TotalPointsEarned,
            Tier = a.Tier,
            TierLabel = a.Tier.ToString(),
            PointsToNextTier = PointsToNextTier(a.TotalPointsEarned),
            PointsValueInCurrency = Math.Round((decimal)a.CurrentPoints / PointsRedemptionRate, 2),
            CreatedAt = a.CreatedAt
        };

        private static LoyaltyTransactionResponse MapTransactionToResponse(LoyaltyTransaction t) => new()
        {
            Id = t.Id,
            Points = t.Points,
            TransactionType = t.TransactionType,
            TypeLabel = t.TransactionType.ToString(),
            ReferenceId = t.ReferenceId,
            Description = t.Description,
            CreatedAt = t.CreatedAt
        };
    }
}
