using HomeTaste.Domain.Entities.Notification;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HomeTaste.Infrastructure.Persistence.Configuration
{
    public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
    {
        public void Configure(EntityTypeBuilder<Notification> builder)
        {
            builder.HasKey(n => n.Id);

            builder.Property(n => n.UserId).HasMaxLength(450);
            builder.Property(n => n.Title).HasMaxLength(200);
            builder.Property(n => n.Message).HasMaxLength(1000);
            builder.Property(n => n.ReferenceType).HasMaxLength(50);

            builder.HasIndex(n => n.UserId);
            builder.HasIndex(n => new { n.UserId, n.IsRead });
        }
    }
}
