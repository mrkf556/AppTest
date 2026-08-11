using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using StoreApp.Domain.Enitities;

namespace StoreApp.Infrastructure.Persistence.Configurations
{
    // این کلاس تنظیمات Mapping مربوط به Basket را در EF Core مشخص می‌کند
    // تنظیمات جدول، کلیدها، Propertyها و ارتباط Basket و BasketItem
    // در این کلاس قرار می‌گیرد تا جزئیات دیتابیس وارد Entityهای Domain نشود
    public class BasketConfiguration : IEntityTypeConfiguration<Basket>
    {
        public void Configure(EntityTypeBuilder<Basket> builder)
        {
            builder.ToTable("Baskets");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.UserId)
                .IsRequired();

            builder.Property(x => x.Status)
                .IsRequired();

            builder.Property(x => x.CreatedAt)
                .IsRequired();

            builder.Property(x => x.LastUpdatedAt)
                .IsRequired(false);

            builder.HasMany(x => x.Items)
                .WithOne()
                .HasForeignKey("BasketId")
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

