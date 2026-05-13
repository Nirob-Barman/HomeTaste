using HomeTaste.Application.DTOs.Analytics;
using HomeTaste.Application.Wrappers;

namespace HomeTaste.Application.Interfaces.Analytics
{
    public interface IAnalyticsService
    {
        Task<Result<DashboardStatsResponse>> GetDashboardStatsAsync();
        Task<Result<List<DailyRevenuePoint>>> GetDailyRevenueAsync(int days = 30);
        Task<Result<List<TopMealItem>>> GetTopMealsAsync(int top = 10);
        Task<Result<List<TopCustomerItem>>> GetTopCustomersAsync(int top = 10);
    }
}
