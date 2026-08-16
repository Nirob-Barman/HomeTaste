using HomeTaste.Domain.Entities;
using HomeTaste.Domain.Entities.Delivery;
using HomeTaste.Domain.Entities.Loyalty;
using HomeTaste.Domain.Entities.MealManagement;
using HomeTaste.Domain.Entities.OrganizationDepartment;
using HomeTaste.Domain.Entities.Payment;
using HomeTaste.Domain.Entities.Support;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using AddressEntity = HomeTaste.Domain.Entities.Address.Address;
using CouponEntity = HomeTaste.Domain.Entities.Coupon.Coupon;
using NotificationEntity = HomeTaste.Domain.Entities.Notification.Notification;
using OrderEntity = HomeTaste.Domain.Entities.Order.Order;
using OrderItemEntity = HomeTaste.Domain.Entities.Order.OrderItem;
using OrderItemCustomizationEntity = HomeTaste.Domain.Entities.Order.OrderItemCustomization;
using TasksEntity = HomeTaste.Domain.Entities.Tasks.Tasks;

namespace HomeTaste.Application.Interfaces.Persistence
{
    /// <summary>
    /// Application-facing abstraction over ApplicationDbContext. Exposes only the entity
    /// DbSets and persistence operations handlers need - no Identity/EF-provider concerns.
    /// </summary>
    public interface IApplicationDbContext
    {
        DbSet<RefreshToken> RefreshTokens { get; }
        DbSet<LoginAudit> LoginAudits { get; }
        DbSet<Units> Units { get; }
        DbSet<MealCategory> MealCategories { get; }
        DbSet<Ingredient> Ingredients { get; }
        DbSet<Meal> Meals { get; }
        DbSet<MealIngredient> MealIngredients { get; }
        DbSet<MealCustomizationOption> MealCustomizationOptions { get; }
        DbSet<TasksEntity> Tasks { get; }
        DbSet<MealReview> MealReviews { get; }
        DbSet<InventoryItem> InventoryItems { get; }
        DbSet<SupportTicket> SupportTickets { get; }
        DbSet<Department> Departments { get; }
        DbSet<CategoryType> CategoryTypes { get; }
        DbSet<InventoryTransaction> InventoryTransactions { get; }
        DbSet<AddressEntity> Addresses { get; }
        DbSet<CouponEntity> Coupons { get; }
        DbSet<OrderEntity> Orders { get; }
        DbSet<OrderItemEntity> OrderItems { get; }
        DbSet<OrderItemCustomizationEntity> OrderItemCustomizations { get; }
        DbSet<PaymentTransaction> PaymentTransactions { get; }
        DbSet<PaymentGateway> PaymentGateways { get; }
        DbSet<DeliveryPersonnel> DeliveryPersonnel { get; }
        DbSet<DeliveryAssignment> DeliveryAssignments { get; }
        DbSet<DeliveryZone> DeliveryZones { get; }
        DbSet<NotificationEntity> Notifications { get; }
        DbSet<LoyaltyAccount> LoyaltyAccounts { get; }
        DbSet<LoyaltyTransaction> LoyaltyTransactions { get; }

        /// <summary>Exposes the underlying DatabaseFacade for explicit transactions where a single SaveChangesAsync isn't enough.</summary>
        DatabaseFacade Database { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
