using HomeTaste.Application.DTOs.Analytics;
using HomeTaste.Application.Interfaces.Analytics;
using HomeTaste.Application.Interfaces.Auth;
using HomeTaste.Application.Interfaces.Persistence;
using HomeTaste.Application.Wrappers;
using HomeTaste.Domain.Entities.Loyalty;
using HomeTaste.Domain.Entities.MealManagement;
using HomeTaste.Domain.Entities.Support;
using HomeTaste.Domain.Enums;
using OrderEntity     = HomeTaste.Domain.Entities.Order.Order;
using OrderItemEntity = HomeTaste.Domain.Entities.Order.OrderItem;

namespace HomeTaste.Application.Services.Analytics
{
    public class AnalyticsService : IAnalyticsService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserManager _userManager;

        public AnalyticsService(IUnitOfWork unitOfWork, IUserManager userManager)
        {
            _unitOfWork  = unitOfWork;
            _userManager = userManager;
        }

        public async Task<Result<DashboardStatsResponse>> GetDashboardStatsAsync()
        {
            var now        = DateTime.UtcNow;
            var todayStart = now.Date;
            var weekStart  = now.AddDays(-7).Date;
            var monthStart = new DateTime(now.Year, now.Month, 1);

            // Orders (project minimal fields — all queries sequential per EF Core scoped-context rule)
            var allOrders = await _unitOfWork.Repository<OrderEntity>().GetAllAsync(
                o => new { o.Status, o.TotalAmount, Date = o.CreatedAt!.Value.Date, o.UserId });

            var delivered = allOrders.Where(o => o.Status == OrderStatus.Delivered).ToList();

            var orderStats = new OrderStatsDto
            {
                TotalAllTime    = allOrders.Count(),
                TodayCount      = allOrders.Count(o => o.Date >= todayStart),
                ThisWeekCount   = allOrders.Count(o => o.Date >= weekStart),
                ThisMonthCount  = allOrders.Count(o => o.Date >= monthStart),
                AverageOrderValue = delivered.Any()
                    ? Math.Round(delivered.Average(o => o.TotalAmount), 2)
                    : 0
            };

            var revenueStats = new RevenueStatsDto
            {
                TotalAllTime = delivered.Sum(o => o.TotalAmount),
                Today        = delivered.Where(o => o.Date >= todayStart).Sum(o => o.TotalAmount),
                ThisWeek     = delivered.Where(o => o.Date >= weekStart).Sum(o => o.TotalAmount),
                ThisMonth    = delivered.Where(o => o.Date >= monthStart).Sum(o => o.TotalAmount)
            };

            var statusBreakdown = Enum.GetValues<OrderStatus>()
                .Select(s => new OrderStatusBreakdownItem
                {
                    Status = s.ToString(),
                    Count  = allOrders.Count(o => o.Status == s)
                }).ToList();

            // Support tickets
            var tickets = await _unitOfWork.Repository<SupportTicket>().GetAllAsync(
                t => new { t.Status });

            var supportSummary = new SupportTicketSummaryDto
            {
                Total      = tickets.Count(),
                Open       = tickets.Count(t => t.Status == TicketStatus.Open),
                InProgress = tickets.Count(t => t.Status == TicketStatus.InProgress),
                Resolved   = tickets.Count(t => t.Status == TicketStatus.Resolved),
                Closed     = tickets.Count(t => t.Status == TicketStatus.Closed)
            };

            // Inventory
            var inventory = await _unitOfWork.Repository<InventoryItem>().GetAllAsync(
                i => new { i.StockCount });

            var inventorySummary = new InventorySummaryDto
            {
                TotalItems     = inventory.Count(),
                LowStockCount  = inventory.Count(i => i.StockCount > 0 && i.StockCount < 10),
                OutOfStockCount = inventory.Count(i => i.StockCount == 0)
            };

            // Loyalty
            var loyaltyTxns = await _unitOfWork.Repository<LoyaltyTransaction>().GetAllAsync(
                t => new { t.TransactionType, t.Points });

            var accountCount = await _unitOfWork.Repository<LoyaltyAccount>().GetAllAsync(a => a.Id);

            var loyaltySummary = new LoyaltySummaryDto
            {
                TotalActiveAccounts  = accountCount.Count(),
                TotalPointsIssued    = loyaltyTxns
                    .Where(t => t.TransactionType == LoyaltyTransactionType.Earned)
                    .Sum(t => t.Points),
                TotalPointsRedeemed  = loyaltyTxns
                    .Where(t => t.TransactionType == LoyaltyTransactionType.Redeemed)
                    .Sum(t => Math.Abs(t.Points))
            };

            // Top meals and customers (sequential — not Task.WhenAll)
            var topMeals     = await GetTopMealsAsync(5);
            var topCustomers = await GetTopCustomersAsync(5);
            var dailyRevenue = await GetDailyRevenueAsync(30);

            return Result<DashboardStatsResponse>.Ok(new DashboardStatsResponse
            {
                Orders               = orderStats,
                Revenue              = revenueStats,
                OrdersByStatus       = statusBreakdown,
                TopMeals             = topMeals.Data     ?? new(),
                TopCustomers         = topCustomers.Data ?? new(),
                DailyRevenueLast30Days = dailyRevenue.Data ?? new(),
                SupportSummary       = supportSummary,
                LoyaltySummary       = loyaltySummary,
                InventorySummary     = inventorySummary
            }, "Dashboard stats retrieved successfully");
        }

        public async Task<Result<List<DailyRevenuePoint>>> GetDailyRevenueAsync(int days = 30)
        {
            var from = DateTime.UtcNow.AddDays(-days).Date;

            var orders = await _unitOfWork.Repository<OrderEntity>().GetAllAsync(
                o => o.Status == OrderStatus.Delivered && o.CreatedAt >= from,
                o => new { o.TotalAmount, Date = o.CreatedAt!.Value.Date });

            var grouped = orders
                .GroupBy(o => o.Date)
                .Select(g => new DailyRevenuePoint
                {
                    Date       = g.Key,
                    Revenue    = g.Sum(o => o.TotalAmount),
                    OrderCount = g.Count()
                })
                .ToDictionary(p => p.Date);

            // Fill every day in the range — missing days get zero values
            var result = Enumerable.Range(0, days)
                .Select(i =>
                {
                    var date = from.AddDays(i);
                    return grouped.TryGetValue(date, out var point)
                        ? point
                        : new DailyRevenuePoint { Date = date };
                })
                .ToList();

            return Result<List<DailyRevenuePoint>>.Ok(result, "Daily revenue retrieved successfully");
        }

        public async Task<Result<List<TopMealItem>>> GetTopMealsAsync(int top = 10)
        {
            var orderItems = await _unitOfWork.Repository<OrderItemEntity>().GetAllAsync(
                oi => new { oi.MealId, oi.Quantity, oi.TotalPrice });

            var grouped = orderItems
                .GroupBy(oi => oi.MealId)
                .Select(g => new
                {
                    MealId               = g.Key,
                    TotalQuantityOrdered = g.Sum(x => x.Quantity),
                    TotalRevenue         = g.Sum(x => x.TotalPrice)
                })
                .OrderByDescending(x => x.TotalQuantityOrdered)
                .Take(top)
                .ToList();

            var result = new List<TopMealItem>();
            foreach (var item in grouped)
            {
                var meal = await _unitOfWork.Repository<Meal>().GetByIdAsync(item.MealId);
                result.Add(new TopMealItem
                {
                    MealId               = item.MealId,
                    MealName             = meal?.Name,
                    TotalQuantityOrdered = item.TotalQuantityOrdered,
                    TotalRevenue         = item.TotalRevenue
                });
            }

            return Result<List<TopMealItem>>.Ok(result, "Top meals retrieved successfully");
        }

        public async Task<Result<List<TopCustomerItem>>> GetTopCustomersAsync(int top = 10)
        {
            var orders = await _unitOfWork.Repository<OrderEntity>().GetAllAsync(
                o => o.Status == OrderStatus.Delivered,
                o => new { o.UserId, o.TotalAmount });

            var grouped = orders
                .GroupBy(o => o.UserId.ToString())
                .Select(g => new
                {
                    UserId      = g.Key,
                    TotalOrders = g.Count(),
                    TotalSpent  = g.Sum(x => x.TotalAmount)
                })
                .OrderByDescending(x => x.TotalSpent)
                .Take(top)
                .ToList();

            var userIds = grouped.Select(x => x.UserId).ToList();
            var users   = (await _userManager.GetUsersByIdsAsync(userIds))
                .ToDictionary(u => u.Id!, u => u);

            var result = grouped.Select(g =>
            {
                users.TryGetValue(g.UserId, out var user);
                var fullName = user == null
                    ? null
                    : $"{user.FirstName} {user.LastName}".Trim();

                return new TopCustomerItem
                {
                    UserId      = g.UserId,
                    FullName    = string.IsNullOrEmpty(fullName) ? null : fullName,
                    Email       = user?.Email,
                    TotalOrders = g.TotalOrders,
                    TotalSpent  = g.TotalSpent
                };
            }).ToList();

            return Result<List<TopCustomerItem>>.Ok(result, "Top customers retrieved successfully");
        }
    }
}
