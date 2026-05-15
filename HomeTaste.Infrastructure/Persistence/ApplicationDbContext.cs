using HomeTaste.Domain.Entities;
using HomeTaste.Domain.Entities.Address;
using HomeTaste.Domain.Entities.Coupon;
using HomeTaste.Domain.Entities.Delivery;
using HomeTaste.Domain.Entities.Loyalty;
using HomeTaste.Domain.Entities.MealManagement;
using HomeTaste.Domain.Entities.Notification;
using HomeTaste.Domain.Entities.Order;
using HomeTaste.Domain.Entities.OrganizationDepartment;
using HomeTaste.Domain.Entities.Payment;
using HomeTaste.Domain.Entities.Support;
using HomeTaste.Domain.Entities.Tasks;
using HomeTaste.Infrastructure.Identity.Entity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace HomeTaste.Infrastructure.Persistence
{
    public class ApplicationDbContext : IdentityDbContext<IdentityApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<LoginAudit> LoginAudits { get; set; }
        public DbSet<Units> Units { get; set; }
        public DbSet<MealCategory> MealCategories { get; set; }
        public DbSet<Ingredient> Ingredients { get; set; }
        public DbSet<Meal> Meals { get; set; }
        public DbSet<MealIngredient> MealIngredients { get; set; }
        public DbSet<MealCustomizationOption> MealCustomizationOptions { get; set; }
        public DbSet<Tasks> Tasks { get; set; }
        public DbSet<MealReview> MealReviews { get; set; }
        public DbSet<InventoryItem> InventoryItems { get; set; }
        public DbSet<SupportTicket> SupportTickets { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<CategoryType> CategoryTypes { get; set; }
        public DbSet<InventoryTransaction> InventoryTransactions { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Coupon> Coupons { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<OrderItemCustomization> OrderItemCustomizations { get; set; }
        public DbSet<PaymentTransaction> PaymentTransactions { get; set; }
        public DbSet<PaymentGateway> PaymentGateways { get; set; }
        public DbSet<DeliveryPersonnel> DeliveryPersonnel { get; set; }
        public DbSet<DeliveryAssignment> DeliveryAssignments { get; set; }
        public DbSet<DeliveryZone> DeliveryZones { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<LoyaltyAccount> LoyaltyAccounts { get; set; }
        public DbSet<LoyaltyTransaction> LoyaltyTransactions { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        }
    }
}
