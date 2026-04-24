using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Products.ListProducts;

public record ListProductsCommand : IRequest<ListProductsResult>
{
    public int Page { get; init; } = 1;
    public int Size { get; init; } = 10;
    public string? Order { get; init; }
    public string? Title { get; init; }
    public string? Category { get; init; }
    public decimal? MinPrice { get; init; }
    public decimal? MaxPrice { get; init; }
}
