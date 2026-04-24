using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Products.CreateProduct;

public record CreateProductCommand : IRequest<CreateProductResult>
{
    public string Title { get; init; } = string.Empty;
    public decimal Price { get; init; }
    public string Description { get; init; } = string.Empty;
    public string Category { get; init; } = string.Empty;
    public string Image { get; init; } = string.Empty;
    public CreateProductRatingCommand Rating { get; init; } = new();
}

public record CreateProductRatingCommand
{
    public decimal Rate { get; init; }
    public int Count { get; init; }
}
