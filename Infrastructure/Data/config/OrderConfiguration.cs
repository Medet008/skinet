using System;
using Core.Entities.OrderAggregate;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.config
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.OwnsOne(o => o.ShipToAddress, a => a.WithOwner());
            // enum to string 209
            builder.Property(s => s.Status).HasConversion(o => o.ToString(), o => (OrderStatus) Enum.Parse(typeof(OrderStatus),o ));
            // Удалить все позиции заказа в заказе
            builder.HasMany(o => o.OrderItems).WithOne().OnDelete(DeleteBehavior.Cascade);
        }
    }
}