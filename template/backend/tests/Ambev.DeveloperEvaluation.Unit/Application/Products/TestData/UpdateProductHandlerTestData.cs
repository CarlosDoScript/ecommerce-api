using Ambev.DeveloperEvaluation.Application.Products.UpdateProduct;
using Bogus;

namespace Ambev.DeveloperEvaluation.Unit.Application.Products.TestData;

/// <summary>
/// Provides methods for generating test data for the UpdateProductHandler.
/// </summary>
public static class UpdateProductHandlerTestData
{
    private static readonly Faker<UpdateProductCommand> _faker = new Faker<UpdateProductCommand>()
        .RuleFor(p => p.Id, f => Guid.NewGuid())
        .RuleFor(p => p.Title, f => f.Commerce.ProductName())
        .RuleFor(p => p.Price, f => f.Random.Decimal(0.01m, 999.99m))
        .RuleFor(p => p.Description, f => f.Commerce.ProductDescription())
        .RuleFor(p => p.Category, f => f.Commerce.Categories(1)[0])
        .RuleFor(p => p.Image, f => f.Internet.Url())
        .RuleFor(p => p.Rating, f => new UpdateProductRatingCommand
        {
            Rate = Math.Round(f.Random.Decimal(0m, 5m), 1),
            Count = f.Random.Int(0, 500)
        });

    public static UpdateProductCommand GenerateValidCommand() => _faker.Generate();

    public static UpdateProductCommand GenerateCommandWithId(Guid id)
    {
        var cmd = _faker.Generate();
        return cmd with { Id = id };
    }
}
