using HomeTaste.Application.Interfaces.Persistence;
using HomeTaste.Domain.Entities;
using HomeTaste.Domain.Entities.Delivery;
using HomeTaste.Domain.Entities.Loyalty;
using HomeTaste.Domain.Entities.MealManagement;
using HomeTaste.Domain.Entities.OrganizationDepartment;
using HomeTaste.Domain.Entities.Payment;
using HomeTaste.Domain.Entities.Support;
using Microsoft.EntityFrameworkCore;
using AddressEntity = HomeTaste.Domain.Entities.Address.Address;
using CouponEntity = HomeTaste.Domain.Entities.Coupon.Coupon;
using NotificationEntity = HomeTaste.Domain.Entities.Notification.Notification;
using OrderEntity = HomeTaste.Domain.Entities.Order.Order;
using OrderItemEntity = HomeTaste.Domain.Entities.Order.OrderItem;
using OrderItemCustomizationEntity = HomeTaste.Domain.Entities.Order.OrderItemCustomization;
using TasksEntity = HomeTaste.Domain.Entities.Tasks.Tasks;

namespace HomeTaste.UnitTests
{
    /// <summary>
    /// Minimal EF Core InMemory-backed IApplicationDbContext for handler unit tests.
    /// Not the real ApplicationDbContext (that lives in Infrastructure and carries Identity
    /// concerns this test project intentionally doesn't reference) - just enough shape to
    /// exercise handlers' LINQ against DbSet&lt;T&gt; with a real (in-memory) query provider,
    /// since DbSet&lt;T&gt;'s async LINQ methods aren't reasonably mockable with Moq.
    /// </summary>
    public class TestApplicationDbContext : DbContext, IApplicationDbContext
    {
        public TestApplicationDbContext(DbContextOptions<TestApplicationDbContext> options) : base(options) { }

        public DbSet<RefreshToken> RefreshTokens { get; set; } = null!;
        public DbSet<LoginAudit> LoginAudits { get; set; } = null!;
        public DbSet<Units> Units { get; set; } = null!;
        public DbSet<MealCategory> MealCategories { get; set; } = null!;
        public DbSet<Ingredient> Ingredients { get; set; } = null!;
        public DbSet<Meal> Meals { get; set; } = null!;
        public DbSet<MealIngredient> MealIngredients { get; set; } = null!;
        public DbSet<MealCustomizationOption> MealCustomizationOptions { get; set; } = null!;
        public DbSet<TasksEntity> Tasks { get; set; } = null!;
        public DbSet<MealReview> MealReviews { get; set; } = null!;
        public DbSet<InventoryItem> InventoryItems { get; set; } = null!;
        public DbSet<SupportTicket> SupportTickets { get; set; } = null!;
        public DbSet<Department> Departments { get; set; } = null!;
        public DbSet<CategoryType> CategoryTypes { get; set; } = null!;
        public DbSet<InventoryTransaction> InventoryTransactions { get; set; } = null!;
        public DbSet<AddressEntity> Addresses { get; set; } = null!;
        public DbSet<CouponEntity> Coupons { get; set; } = null!;
        public DbSet<OrderEntity> Orders { get; set; } = null!;
        public DbSet<OrderItemEntity> OrderItems { get; set; } = null!;
        public DbSet<OrderItemCustomizationEntity> OrderItemCustomizations { get; set; } = null!;
        public DbSet<PaymentTransaction> PaymentTransactions { get; set; } = null!;
        public DbSet<PaymentGateway> PaymentGateways { get; set; } = null!;
        public DbSet<DeliveryPersonnel> DeliveryPersonnel { get; set; } = null!;
        public DbSet<DeliveryAssignment> DeliveryAssignments { get; set; } = null!;
        public DbSet<DeliveryZone> DeliveryZones { get; set; } = null!;
        public DbSet<NotificationEntity> Notifications { get; set; } = null!;
        public DbSet<LoyaltyAccount> LoyaltyAccounts { get; set; } = null!;
        public DbSet<LoyaltyTransaction> LoyaltyTransactions { get; set; } = null!;

        public static TestApplicationDbContext CreateInMemory(string? databaseName = null)
        {
            var options = new DbContextOptionsBuilder<TestApplicationDbContext>()
                .UseInMemoryDatabase(databaseName ?? Guid.NewGuid().ToString())
                .Options;

            return new TestApplicationDbContext(options);
        }
    }
}
