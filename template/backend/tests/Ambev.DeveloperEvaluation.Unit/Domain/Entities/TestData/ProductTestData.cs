using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.ValueObjects;
using Bogus;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;

/// <summary>
/// Provides methods for generating test data for the Product entity.
/// Centralizes all product test data generation to ensure consistency across test cases.
/// </summary>
public static class ProductTestData
{
    private static readonly Faker<Product> ProductFaker = new Faker<Product>()
        .RuleFor(p => p.Id, f => Guid.NewGuid())
        .RuleFor(p => p.Title, f => f.Commerce.ProductName())
        .RuleFor(p => p.Price, f => f.Random.Decimal(0.01m, 9999.99m))
        .RuleFor(p => p.Description, f => f.Commerce.ProductDescription())
        .RuleFor(p => p.Category, f => f.Commerce.Categories(1)[0])
        .RuleFor(p => p.Image, f => f.Internet.Url())
        .RuleFor(p => p.Rating, f => new ProductRating
        {
            Rate = Math.Round(f.Random.Decimal(0m, 5m), 1),
            Count = f.Random.Int(0, 1000)
        })
        .RuleFor(p => p.CreatedAt, f => f.Date.Past());

    /// <summary>
    /// Generates a valid Product entity with randomized data.
    /// </summary>
    public static Product GenerateValidProduct() => ProductFaker.Generate();

    /// <summary>
    /// Generates a valid product title.
    /// </summary>
    public static string GenerateValidTitle() => new Faker().Commerce.ProductName();

    /// <summary>
    /// Generates a valid positive price.
    /// </summary>
    public static decimal GenerateValidPrice() => Math.Round(new Faker().Random.Decimal(0.01m, 999.99m), 2);

    /// <summary>
    /// Generates a valid category name.
    /// </summary>
    public static string GenerateValidCategory() => new Faker().Commerce.Categories(1)[0];

    /// <summary>
    /// Generates an invalid price (zero or negative).
    /// </summary>
    public static decimal GenerateInvalidPrice() => new Faker().Random.Decimal(-100m, 0m);

    /// <summary>
    /// Generates a title that exceeds the maximum length of 200 characters.
    /// </summary>
    public static string GenerateLongTitle() => new Faker().Random.String2(201);

    /// <summary>
    /// Generates a category that exceeds the maximum length of 100 characters.
    /// </summary>
    public static string GenerateLongCategory() => new Faker().Random.String2(101);

    /// <summary>
    /// Generates an invalid rating rate (above 5).
    /// </summary>
    public static decimal GenerateInvalidRate() => new Faker().Random.Decimal(5.1m, 10m);

    /// <summary>
    /// Generates an invalid rating count (negative).
    /// </summary>
    public static int GenerateInvalidCount() => new Faker().Random.Int(-100, -1);
}
