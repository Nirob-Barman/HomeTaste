namespace HomeTaste.Application.DTOs.Analytics
{
    public class DashboardStatsResponse
    {
        public OrderStatsDto Orders { get; set; } = new();
        public RevenueStatsDto Revenue { get; set; } = new();
        public List<OrderStatusBreakdownItem> OrdersByStatus { get; set; } = new();
        public List<TopMealItem> TopMeals { get; set; } = new();
        public List<TopCustomerItem> TopCustomers { get; set; } = new();
        public List<DailyRevenuePoint> DailyRevenueLast30Days { get; set; } = new();
        public SupportTicketSummaryDto SupportSummary { get; set; } = new();
        public LoyaltySummaryDto LoyaltySummary { get; set; } = new();
        public InventorySummaryDto InventorySummary { get; set; } = new();
    }

    public class OrderStatsDto
    {
        public int TotalAllTime { get; set; }
        public int TodayCount { get; set; }
        public int ThisWeekCount { get; set; }
        public int ThisMonthCount { get; set; }
        public decimal AverageOrderValue { get; set; }
    }

    public class RevenueStatsDto
    {
        public decimal TotalAllTime { get; set; }
        public decimal Today { get; set; }
        public decimal ThisWeek { get; set; }
        public decimal ThisMonth { get; set; }
    }

    public class OrderStatusBreakdownItem
    {
        public string Status { get; set; } = string.Empty;
        public int Count { get; set; }
    }

    public class TopMealItem
    {
        public Guid MealId { get; set; }
        public string? MealName { get; set; }
        public int TotalQuantityOrdered { get; set; }
        public decimal TotalRevenue { get; set; }
    }

    public class TopCustomerItem
    {
        public string UserId { get; set; } = string.Empty;
        public string? FullName { get; set; }
        public string? Email { get; set; }
        public int TotalOrders { get; set; }
        public decimal TotalSpent { get; set; }
    }

    public class DailyRevenuePoint
    {
        public DateTime Date { get; set; }
        public decimal Revenue { get; set; }
        public int OrderCount { get; set; }
    }

    public class SupportTicketSummaryDto
    {
        public int Total { get; set; }
        public int Open { get; set; }
        public int InProgress { get; set; }
        public int Resolved { get; set; }
        public int Closed { get; set; }
    }

    public class LoyaltySummaryDto
    {
        public int TotalActiveAccounts { get; set; }
        public int TotalPointsIssued { get; set; }
        public int TotalPointsRedeemed { get; set; }
    }

    public class InventorySummaryDto
    {
        public int TotalItems { get; set; }
        public int LowStockCount { get; set; }
        public int OutOfStockCount { get; set; }
    }
}
