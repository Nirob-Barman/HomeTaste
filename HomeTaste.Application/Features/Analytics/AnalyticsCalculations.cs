using HomeTaste.Application.Interfaces.Auth;
using HomeTaste.Application.Interfaces.Persistence;
using HomeTaste.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace HomeTaste.Application.Features.Analytics
{
    public static class AnalyticsCalculations
    {
        public static async Task<List<DailyRevenuePoint>> GetDailyRevenueAsync(IApplicationDbContext context, int days, CancellationToken cancellationToken)
        {
            var from = DateTime.UtcNow.AddDays(-days).Date;

            var orders = await context.Orders
                .Where(o => o.Status == OrderStatus.Delivered && o.CreatedAt >= from)
                .Select(o => new { o.TotalAmount, Date = o.CreatedAt!.Value.Date })
                .ToListAsync(cancellationToken);

            var grouped = orders
                .GroupBy(o => o.Date)
                .Select(g => new DailyRevenuePoint
                {
                    Date = g.Key,
                    Revenue = g.Sum(o => o.TotalAmount),
                    OrderCount = g.Count()
                })
                .ToDictionary(p => p.Date);

            return Enumerable.Range(0, days)
                .Select(i =>
                {
                    var date = from.AddDays(i);
                    return grouped.TryGetValue(date, out var point)
                        ? point
                        : new DailyRevenuePoint { Date = date };
                })
                .ToList();
        }

        public static async Task<List<TopMealItem>> GetTopMealsAsync(IApplicationDbContext context, int top, CancellationToken cancellationToken)
        {
            var orderItems = await context.OrderItems
                .Select(oi => new { oi.MealId, oi.Quantity, oi.TotalPrice })
                .ToListAsync(cancellationToken);

            var grouped = orderItems
                .GroupBy(oi => oi.MealId)
                .Select(g => new
                {
                    MealId = g.Key,
                    TotalQuantityOrdered = g.Sum(x => x.Quantity),
                    TotalRevenue = g.Sum(x => x.TotalPrice)
                })
                .OrderByDescending(x => x.TotalQuantityOrdered)
                .Take(top)
                .ToList();

            var result = new List<TopMealItem>();
            foreach (var item in grouped)
            {
                var meal = await context.Meals.FindAsync(new object?[] { item.MealId }, cancellationToken);
                result.Add(new TopMealItem
                {
                    MealId = item.MealId,
                    MealName = meal?.Name,
                    TotalQuantityOrdered = item.TotalQuantityOrdered,
                    TotalRevenue = item.TotalRevenue
                });
            }

            return result;
        }

        public static async Task<List<TopCustomerItem>> GetTopCustomersAsync(IApplicationDbContext context, IUserManager userManager, int top, CancellationToken cancellationToken)
        {
            var orders = await context.Orders
                .Where(o => o.Status == OrderStatus.Delivered)
                .Select(o => new { o.UserId, o.TotalAmount })
                .ToListAsync(cancellationToken);

            var grouped = orders
                .GroupBy(o => o.UserId.ToString())
                .Select(g => new
                {
                    UserId = g.Key,
                    TotalOrders = g.Count(),
                    TotalSpent = g.Sum(x => x.TotalAmount)
                })
                .OrderByDescending(x => x.TotalSpent)
                .Take(top)
                .ToList();

            var userIds = grouped.Select(x => x.UserId).ToList();
            var users = (await userManager.GetUsersByIdsAsync(userIds))
                .ToDictionary(u => u.Id!, u => u);

            return grouped.Select(g =>
            {
                users.TryGetValue(g.UserId, out var user);
                var fullName = user == null
                    ? null
                    : $"{user.FirstName} {user.LastName}".Trim();

                return new TopCustomerItem
                {
                    UserId = g.UserId,
                    FullName = string.IsNullOrEmpty(fullName) ? null : fullName,
                    Email = user?.Email,
                    TotalOrders = g.TotalOrders,
                    TotalSpent = g.TotalSpent
                };
            }).ToList();
        }
    }
}
