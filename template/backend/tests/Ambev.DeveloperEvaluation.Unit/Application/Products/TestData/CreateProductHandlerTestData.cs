using Ambev.DeveloperEvaluation.Application.Products.CreateProduct;
using Bogus;

namespace Ambev.DeveloperEvaluation.Unit.Application.Products.TestData;

public static class CreateProductHandlerTestData
{
    private static readonly Faker<CreateProductCommand> _faker = new Faker<CreateProductCommand>()
        .RuleFor(p => p.Title, f => f.Commerce.ProductName())
        .RuleFor(p => p.Price, f => f.Random.Decimal(1, 1000))
        .RuleFor(p => p.Description, f => f.Commerce.ProductDescription())
        .RuleFor(p => p.Category, f => f.Commerce.Categories(1)[0])
        .RuleFor(p => p.Image, f => f.Internet.Url())
        .RuleFor(p => p.Rating, f => new CreateProductRatingCommand
        {
            Rate = f.Random.Decimal(0, 5),
            Count = f.Random.Int(0, 500)
        });

    public static CreateProductCommand GenerateValidCommand() => _faker.Generate();
}
