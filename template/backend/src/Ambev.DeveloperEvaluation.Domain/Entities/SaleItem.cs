namespace Ambev.DeveloperEvaluation.Domain.Entities;

public class SaleItem
{
    // External Identity — denormalized at time of sale
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }

    public int Quantity { get; set; }
    public decimal Discount { get; set; }
    public decimal TotalAmount { get; set; }
    public bool IsCancelled { get; set; }

    /// <summary>
    /// Applies quantity-based discount rules and recalculates TotalAmount.
    /// Rules:
    ///   Qty  1-3  → 0%  discount
    ///   Qty  4-9  → 10% discount
    ///   Qty 10-20 → 20% discount
    ///   Qty > 20  → not allowed (throws)
    /// </summary>
    public void ApplyBusinessRules()
    {
        if (Quantity > 20)
            throw new InvalidOperationException(
                $"Cannot sell more than 20 identical items. Product: '{ProductName}', requested: {Quantity}.");

        Discount = Quantity switch
        {
            >= 10 => 0.20m,
            >= 4  => 0.10m,
            _     => 0m
        };

        TotalAmount = Math.Round(UnitPrice * Quantity * (1 - Discount), 2);
    }
}
