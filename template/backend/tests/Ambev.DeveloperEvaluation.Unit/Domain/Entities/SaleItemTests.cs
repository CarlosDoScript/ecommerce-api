using Ambev.DeveloperEvaluation.Domain.Entities;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Entities;

/// <summary>
/// Unit tests for SaleItem business rules (discount tiers and restrictions).
/// These are the core business rules evaluated in the technical test.
/// </summary>
public class SaleItemTests
{
    [Theory(DisplayName = "Quantity below 4 should have 0% discount")]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public void Given_QuantityBelow4_When_ApplyBusinessRules_Then_NoDiscount(int quantity)
    {
        // Given
        var item = new SaleItem { ProductId = Guid.NewGuid(), ProductName = "Product", UnitPrice = 100m, Quantity = quantity };

        // When
        item.ApplyBusinessRules();

        // Then
        item.Discount.Should().Be(0m);
        item.TotalAmount.Should().Be(item.UnitPrice * quantity);
    }

    [Theory(DisplayName = "Quantity between 4 and 9 should have 10% discount")]
    [InlineData(4)]
    [InlineData(5)]
    [InlineData(9)]
    public void Given_QuantityBetween4And9_When_ApplyBusinessRules_Then_10PercentDiscount(int quantity)
    {
        // Given
        var item = new SaleItem { ProductId = Guid.NewGuid(), ProductName = "Product", UnitPrice = 100m, Quantity = quantity };

        // When
        item.ApplyBusinessRules();

        // Then
        item.Discount.Should().Be(0.10m);
        item.TotalAmount.Should().Be(Math.Round(100m * quantity * 0.90m, 2));
    }

    [Theory(DisplayName = "Quantity between 10 and 20 should have 20% discount")]
    [InlineData(10)]
    [InlineData(15)]
    [InlineData(20)]
    public void Given_QuantityBetween10And20_When_ApplyBusinessRules_Then_20PercentDiscount(int quantity)
    {
        // Given
        var item = new SaleItem { ProductId = Guid.NewGuid(), ProductName = "Product", UnitPrice = 100m, Quantity = quantity };

        // When
        item.ApplyBusinessRules();

        // Then
        item.Discount.Should().Be(0.20m);
        item.TotalAmount.Should().Be(Math.Round(100m * quantity * 0.80m, 2));
    }

    [Theory(DisplayName = "Quantity above 20 should throw InvalidOperationException")]
    [InlineData(21)]
    [InlineData(50)]
    [InlineData(100)]
    public void Given_QuantityAbove20_When_ApplyBusinessRules_Then_ThrowsException(int quantity)
    {
        // Given
        var item = new SaleItem { ProductId = Guid.NewGuid(), ProductName = "Product", UnitPrice = 100m, Quantity = quantity };

        // When
        var act = () => item.ApplyBusinessRules();

        // Then
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*20*");
    }

    [Fact(DisplayName = "TotalAmount should be correctly calculated with 10% discount")]
    public void Given_Quantity5_UnitPrice200_When_ApplyBusinessRules_Then_TotalIsCorrect()
    {
        // Given
        var item = new SaleItem { ProductId = Guid.NewGuid(), ProductName = "Widget", UnitPrice = 200m, Quantity = 5 };

        // When
        item.ApplyBusinessRules();

        // Then
        item.Discount.Should().Be(0.10m);
        item.TotalAmount.Should().Be(900m); // 200 * 5 * 0.90 = 900
    }

    [Fact(DisplayName = "TotalAmount should be correctly calculated with 20% discount")]
    public void Given_Quantity10_UnitPrice50_When_ApplyBusinessRules_Then_TotalIsCorrect()
    {
        // Given
        var item = new SaleItem { ProductId = Guid.NewGuid(), ProductName = "Widget", UnitPrice = 50m, Quantity = 10 };

        // When
        item.ApplyBusinessRules();

        // Then
        item.Discount.Should().Be(0.20m);
        item.TotalAmount.Should().Be(400m); // 50 * 10 * 0.80 = 400
    }

    [Fact(DisplayName = "Exact boundary at 4 items should apply 10% discount")]
    public void Given_ExactlyFourItems_When_ApplyBusinessRules_Then_10PercentDiscount()
    {
        // Given — lower boundary: first tier to qualify for discount
        var item = new SaleItem { ProductId = Guid.NewGuid(), ProductName = "Product", UnitPrice = 100m, Quantity = 4 };

        // When
        item.ApplyBusinessRules();

        // Then
        item.Discount.Should().Be(0.10m);
    }

    [Fact(DisplayName = "Exact boundary at 10 items should apply 20% discount")]
    public void Given_ExactlyTenItems_When_ApplyBusinessRules_Then_20PercentDiscount()
    {
        // Given — lower boundary: first quantity to hit 20% tier
        var item = new SaleItem { ProductId = Guid.NewGuid(), ProductName = "Product", UnitPrice = 100m, Quantity = 10 };

        // When
        item.ApplyBusinessRules();

        // Then
        item.Discount.Should().Be(0.20m);
    }

    [Fact(DisplayName = "Exact boundary at 20 items should be allowed with 20% discount")]
    public void Given_ExactlyTwentyItems_When_ApplyBusinessRules_Then_AllowedWith20PercentDiscount()
    {
        // Given — upper boundary: last allowed quantity
        var item = new SaleItem { ProductId = Guid.NewGuid(), ProductName = "Product", UnitPrice = 100m, Quantity = 20 };

        // When
        item.ApplyBusinessRules();

        // Then
        item.Discount.Should().Be(0.20m);
        item.TotalAmount.Should().Be(1600m); // 100 * 20 * 0.80 = 1600
    }
}
