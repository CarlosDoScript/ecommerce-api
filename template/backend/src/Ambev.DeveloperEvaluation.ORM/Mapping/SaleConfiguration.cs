using Ambev.DeveloperEvaluation.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ambev.DeveloperEvaluation.ORM.Mapping;

public class SaleConfiguration : IEntityTypeConfiguration<Sale>
{
    public void Configure(EntityTypeBuilder<Sale> builder)
    {
        builder.ToTable("Sales");

        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id).HasColumnType("uuid").HasDefaultValueSql("gen_random_uuid()");

        builder.Property(s => s.SaleNumber).HasMaxLength(50).IsRequired();
        builder.Property(s => s.Date).IsRequired();
        builder.Property(s => s.CustomerId).IsRequired();
        builder.Property(s => s.CustomerName).HasMaxLength(100).IsRequired();
        builder.Property(s => s.BranchId).IsRequired();
        builder.Property(s => s.BranchName).HasMaxLength(100).IsRequired();
        builder.Property(s => s.TotalAmount).HasColumnType("numeric(18,2)").IsRequired();
        builder.Property(s => s.IsCancelled).IsRequired();
        builder.Property(s => s.CreatedAt).IsRequired();
        builder.Property(s => s.UpdatedAt);

        builder.OwnsMany(s => s.Items, item =>
        {
            item.ToTable("SaleItems");
            item.WithOwner().HasForeignKey("SaleId");
            item.Property<Guid>("SaleId");
            item.Property(i => i.ProductId).IsRequired();
            item.Property(i => i.ProductName).HasMaxLength(200).IsRequired();
            item.Property(i => i.UnitPrice).HasColumnType("numeric(18,2)").IsRequired();
            item.Property(i => i.Quantity).IsRequired();
            item.Property(i => i.Discount).HasColumnType("numeric(5,4)").IsRequired();
            item.Property(i => i.TotalAmount).HasColumnType("numeric(18,2)").IsRequired();
            item.Property(i => i.IsCancelled).IsRequired();
        });
    }
}
