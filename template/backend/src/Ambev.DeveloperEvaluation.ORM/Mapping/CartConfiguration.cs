using Ambev.DeveloperEvaluation.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ambev.DeveloperEvaluation.ORM.Mapping;

public class CartConfiguration : IEntityTypeConfiguration<Cart>
{
    public void Configure(EntityTypeBuilder<Cart> builder)
    {
        builder.ToTable("Carts");

        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id).HasColumnType("uuid").HasDefaultValueSql("gen_random_uuid()");

        builder.Property(c => c.UserId).HasColumnType("uuid").IsRequired();
        builder.Property(c => c.Date).HasColumnType("timestamp with time zone").IsRequired();
        builder.Property(c => c.CreatedAt).HasColumnType("timestamp with time zone").IsRequired();
        builder.Property(c => c.UpdatedAt).HasColumnType("timestamp with time zone");

        builder.OwnsMany(c => c.Products, item =>
        {
            item.ToTable("CartItems");
            item.WithOwner().HasForeignKey("CartId");
            item.Property<Guid>("CartId").HasColumnType("uuid");
            item.Property(i => i.ProductId).HasColumnType("uuid").IsRequired();
            item.Property(i => i.Quantity).HasColumnType("integer").IsRequired();
            item.HasKey("CartId", "ProductId");
        });
    }
}
