using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Sales.ListSales;

public record ListSalesCommand : IRequest<ListSalesResult>
{
    public int Page { get; init; } = 1;
    public int Size { get; init; } = 10;
    public string? Order { get; init; }
    public Guid? CustomerId { get; init; }
    public Guid? BranchId { get; init; }
    public bool? IsCancelled { get; init; }
    public DateTime? MinDate { get; init; }
    public DateTime? MaxDate { get; init; }
}
