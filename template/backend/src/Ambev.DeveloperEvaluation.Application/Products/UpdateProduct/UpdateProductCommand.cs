using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Products.UpdateProduct;

public record UpdateProductCommand : IRequest<UpdateProductResult>
{
    public Guid Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public decimal Price { get; init; }
    public string Description { get; init; } = string.Empty;
    public string Category { get; init; } = string.Empty;
    public string Image { get; init; } = string.Empty;
    public UpdateProductRatingCommand Rating { get; init; } = new();
}

public record UpdateProductRatingCommand
{
    public decimal Rate { get; init; }
    public int Count { get; init; }
}
