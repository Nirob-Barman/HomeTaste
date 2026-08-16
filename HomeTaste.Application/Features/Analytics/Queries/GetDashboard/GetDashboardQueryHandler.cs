using HomeTaste.Application.Interfaces.Auth;
using HomeTaste.Application.Interfaces.Persistence;
using HomeTaste.Application.Wrappers;
using HomeTaste.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HomeTaste.Application.Features.Analytics.Queries.GetDashboard
{
    public class GetDashboardQueryHandler : IRequestHandler<GetDashboardQuery, Result<DashboardStatsResponse>>
    {
        private readonly IApplicationDbContext _context;
        private readonly IUserManager _userManager;

        public GetDashboardQueryHandler(IApplicationDbContext context, IUserManager userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<Result<DashboardStatsResponse>> Handle(GetDashboardQuery request, CancellationToken cancellationToken)
        {
            var now = DateTime.UtcNow;
            var todayStart = now.Date;
            var weekStart = now.AddDays(-7).Date;
            var monthStart = new DateTime(now.Year, now.Month, 1);

            // Orders (project minimal fields — all queries sequential per EF Core scoped-context rule)
            var allOrders = await _context.Orders
                .Select(o => new { o.Status, o.TotalAmount, Date = o.CreatedAt!.Value.Date, o.UserId })
                .ToListAsync(cancellationToken);

            var delivered = allOrders.Where(o => o.Status == OrderStatus.Delivered).ToList();

            var orderStats = new OrderStatsDto
            {
                TotalAllTime = allOrders.Count,
                TodayCount = allOrders.Count(o => o.Date >= todayStart),
                ThisWeekCount = allOrders.Count(o => o.Date >= weekStart),
                ThisMonthCount = allOrders.Count(o => o.Date >= monthStart),
                AverageOrderValue = delivered.Any()
                    ? Math.Round(delivered.Average(o => o.TotalAmount), 2)
                    : 0
            };

            var revenueStats = new RevenueStatsDto
            {
                TotalAllTime = delivered.Sum(o => o.TotalAmount),
                Today = delivered.Where(o => o.Date >= todayStart).Sum(o => o.TotalAmount),
                ThisWeek = delivered.Where(o => o.Date >= weekStart).Sum(o => o.TotalAmount),
                ThisMonth = delivered.Where(o => o.Date >= monthStart).Sum(o => o.TotalAmount)
            };

            var statusBreakdown = Enum.GetValues<OrderStatus>()
                .Select(s => new OrderStatusBreakdownItem
                {
                    Status = s.ToString(),
                    Count = allOrders.Count(o => o.Status == s)
                }).ToList();

            // Support tickets
            var tickets = await _context.SupportTickets
                .Select(t => new { t.Status })
                .ToListAsync(cancellationToken);

            var supportSummary = new SupportTicketSummaryDto
            {
                Total = tickets.Count,
                Open = tickets.Count(t => t.Status == TicketStatus.Open),
                InProgress = tickets.Count(t => t.Status == TicketStatus.InProgress),
                Resolved = tickets.Count(t => t.Status == TicketStatus.Resolved),
                Closed = tickets.Count(t => t.Status == TicketStatus.Closed)
            };

            // Inventory
            var inventory = await _context.InventoryItems
                .Select(i => new { i.StockCount })
                .ToListAsync(cancellationToken);

            var inventorySummary = new InventorySummaryDto
            {
                TotalItems = inventory.Count,
                LowStockCount = inventory.Count(i => i.StockCount > 0 && i.StockCount < 10),
                OutOfStockCount = inventory.Count(i => i.StockCount == 0)
            };

            // Loyalty
            var loyaltyTxns = await _context.LoyaltyTransactions
                .Select(t => new { t.TransactionType, t.Points })
                .ToListAsync(cancellationToken);

            var accountCount = await _context.LoyaltyAccounts.CountAsync(cancellationToken);

            var loyaltySummary = new LoyaltySummaryDto
            {
                TotalActiveAccounts = accountCount,
                TotalPointsIssued = loyaltyTxns
                    .Where(t => t.TransactionType == LoyaltyTransactionType.Earned)
                    .Sum(t => t.Points),
                TotalPointsRedeemed = loyaltyTxns
                    .Where(t => t.TransactionType == LoyaltyTransactionType.Redeemed)
                    .Sum(t => Math.Abs(t.Points))
            };

            // Top meals and customers (sequential — not Task.WhenAll)
            var topMeals = await AnalyticsCalculations.GetTopMealsAsync(_context, 5, cancellationToken);
            var topCustomers = await AnalyticsCalculations.GetTopCustomersAsync(_context, _userManager, 5, cancellationToken);
            var dailyRevenue = await AnalyticsCalculations.GetDailyRevenueAsync(_context, 30, cancellationToken);

            return Result<DashboardStatsResponse>.Ok(new DashboardStatsResponse
            {
                Orders = orderStats,
                Revenue = revenueStats,
                OrdersByStatus = statusBreakdown,
                TopMeals = topMeals,
                TopCustomers = topCustomers,
                DailyRevenueLast30Days = dailyRevenue,
                SupportSummary = supportSummary,
                LoyaltySummary = loyaltySummary,
                InventorySummary = inventorySummary
            }, "Dashboard stats retrieved successfully");
        }
    }
}
