using Ambev.DeveloperEvaluation.Domain.Common;

namespace Ambev.DeveloperEvaluation.Domain.Entities;

public class Sale : BaseEntity
{
    public string SaleNumber { get; set; } = string.Empty;
    public DateTime Date { get; set; }

    // External Identity — Customer (denormalized)
    public Guid CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;

    // External Identity — Branch (denormalized)
    public Guid BranchId { get; set; }
    public string BranchName { get; set; } = string.Empty;

    public decimal TotalAmount { get; set; }
    public bool IsCancelled { get; set; }

    public List<SaleItem> Items { get; set; } = new();

    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public Sale()
    {
        CreatedAt = DateTime.UtcNow;
        SaleNumber = GenerateSaleNumber();
    }

    /// <summary>
    /// Applies business rules to all active items and recalculates the sale total.
    /// </summary>
    public void CalculateTotals()
    {
        foreach (var item in Items.Where(i => !i.IsCancelled))
            item.ApplyBusinessRules();

        TotalAmount = Items
            .Where(i => !i.IsCancelled)
            .Sum(i => i.TotalAmount);
    }

    /// <summary>
    /// Cancels the entire sale and all its items.
    /// </summary>
    public void Cancel()
    {
        IsCancelled = true;
        UpdatedAt = DateTime.UtcNow;
    }

    private static string GenerateSaleNumber() =>
        $"{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N")[..8].ToUpper()}";
}
