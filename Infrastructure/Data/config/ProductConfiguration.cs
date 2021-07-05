using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.config
{
  public class ProductConfiguration : IEntityTypeConfiguration<Product>
  {
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.Property(p => p.Id).IsRequired();
        builder.Property(p => p.Name).IsRequired().HasMaxLength(100);
        builder.Property(p => p.Description).IsRequired();
        // Количество десятичных знаков должно быть до двух
        builder.Property(p => p.Price).HasColumnType("decimal(18,2)");
        //builder.Property(p => p.PictureUrl) .IsRequired ();
        // у продукта один бренд, но бренд может быть связан со многими продуктами
        builder.HasOne(b => b.ProductBrand).WithMany().HasForeignKey(p => p.ProductBrandId);
        builder.HasOne(t => t.ProductType).WithMany().HasForeignKey(p => p.ProductTypeId);
    }
  }
}