using Ambev.DeveloperEvaluation.Domain.Entities;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Entities;

/// <summary>
/// Unit tests for Sale aggregate business logic (total calculation, cancellation).
/// </summary>
public class SaleTests
{
    [Fact(DisplayName = "CalculateTotals should sum only non-cancelled items")]
    public void Given_SaleWithMixedItems_When_CalculateTotals_Then_TotalExcludesCancelledItems()
    {
        // Given
        var sale = new Sale
        {
            Items =
            [
                new SaleItem { ProductId = Guid.NewGuid(), ProductName = "A", UnitPrice = 100m, Quantity = 2 },
                new SaleItem { ProductId = Guid.NewGuid(), ProductName = "B", UnitPrice = 50m, Quantity = 3, IsCancelled = true },
                new SaleItem { ProductId = Guid.NewGuid(), ProductName = "C", UnitPrice = 200m, Quantity = 1 }
            ]
        };

        // When
        sale.CalculateTotals();

        // Then — B is cancelled (50*3=150) and should not be included
        sale.TotalAmount.Should().Be(400m); // 100*2 + 200*1 = 400 (both without discount)
    }

    [Fact(DisplayName = "CalculateTotals with items qualifying for discount should reflect discounted total")]
    public void Given_SaleWithDiscountableItems_When_CalculateTotals_Then_TotalReflectsDiscounts()
    {
        // Given — 5 items at $100 → 10% discount → $450 each group
        var sale = new Sale
        {
            Items =
            [
                new SaleItem { ProductId = Guid.NewGuid(), ProductName = "Widget", UnitPrice = 100m, Quantity = 5 },
                new SaleItem { ProductId = Guid.NewGuid(), ProductName = "Gadget", UnitPrice = 50m, Quantity = 10 }
            ]
        };

        // When
        sale.CalculateTotals();

        // Then
        // Widget: 100 * 5 * 0.90 = 450
        // Gadget: 50 * 10 * 0.80 = 400
        sale.TotalAmount.Should().Be(850m);
    }

    [Fact(DisplayName = "Cancel should set IsCancelled to true and set UpdatedAt")]
    public void Given_ActiveSale_When_Cancel_Then_SaleIsCancelled()
    {
        // Given
        var sale = new Sale { IsCancelled = false };
        var before = DateTime.UtcNow;

        // When
        sale.Cancel();

        // Then
        sale.IsCancelled.Should().BeTrue();
        sale.UpdatedAt.Should().NotBeNull();
        sale.UpdatedAt.Should().BeOnOrAfter(before);
    }

    [Fact(DisplayName = "New Sale should auto-generate SaleNumber")]
    public void Given_NewSale_When_Created_Then_SaleNumberIsGenerated()
    {
        // When
        var sale = new Sale();

        // Then
        sale.SaleNumber.Should().NotBeNullOrEmpty();
        sale.SaleNumber.Should().MatchRegex(@"^\d{8}-[A-F0-9]{8}$");
    }

    [Fact(DisplayName = "CalculateTotals should throw when an item exceeds 20 units")]
    public void Given_ItemWithQuantityAbove20_When_CalculateTotals_Then_ThrowsException()
    {
        // Given
        var sale = new Sale
        {
            Items = [new SaleItem { ProductId = Guid.NewGuid(), ProductName = "Product", UnitPrice = 10m, Quantity = 21 }]
        };

        // When
        var act = () => sale.CalculateTotals();

        // Then
        act.Should().Throw<InvalidOperationException>().WithMessage("*20*");
    }

    [Fact(DisplayName = "CalculateTotals with all items cancelled should result in zero total")]
    public void Given_SaleWithAllItemsCancelled_When_CalculateTotals_Then_TotalIsZero()
    {
        // Given — all items cancelled, none should contribute to total
        var sale = new Sale
        {
            Items =
            [
                new SaleItem { ProductId = Guid.NewGuid(), ProductName = "A", UnitPrice = 100m, Quantity = 5, IsCancelled = true },
                new SaleItem { ProductId = Guid.NewGuid(), ProductName = "B", UnitPrice = 200m, Quantity = 10, IsCancelled = true }
            ]
        };

        // When
        sale.CalculateTotals();

        // Then
        sale.TotalAmount.Should().Be(0m);
    }

    [Fact(DisplayName = "CalculateTotals with no items should result in zero total")]
    public void Given_SaleWithNoItems_When_CalculateTotals_Then_TotalIsZero()
    {
        // Given
        var sale = new Sale { Items = [] };

        // When
        sale.CalculateTotals();

        // Then
        sale.TotalAmount.Should().Be(0m);
    }
}
