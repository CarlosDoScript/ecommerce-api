using Ambev.DeveloperEvaluation.Domain.Entities;
using Bogus;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;

/// <summary>
/// Provides methods for generating test data for the Cart entity.
/// Centralizes all cart test data generation to ensure consistency across test cases.
/// </summary>
public static class CartTestData
{
    private static readonly Faker<CartItem> CartItemFaker = new Faker<CartItem>()
        .RuleFor(i => i.ProductId, f => Guid.NewGuid())
        .RuleFor(i => i.Quantity, f => f.Random.Int(1, 20));

    private static readonly Faker<Cart> CartFaker = new Faker<Cart>()
        .RuleFor(c => c.Id, f => Guid.NewGuid())
        .RuleFor(c => c.UserId, f => Guid.NewGuid())
        .RuleFor(c => c.Date, f => f.Date.Recent())
        .RuleFor(c => c.Products, f => CartItemFaker.Generate(f.Random.Int(1, 5)))
        .RuleFor(c => c.CreatedAt, f => f.Date.Past());

    /// <summary>
    /// Generates a valid Cart entity with randomized data.
    /// </summary>
    public static Cart GenerateValidCart() => CartFaker.Generate();

    /// <summary>
    /// Generates a list of valid CartItem instances.
    /// </summary>
    public static List<CartItem> GenerateValidCartItems(int count = 2) =>
        CartItemFaker.Generate(count);

    /// <summary>
    /// Generates a CartItem with an invalid quantity (zero or negative).
    /// </summary>
    public static CartItem GenerateInvalidCartItem() =>
        new CartItem { ProductId = Guid.NewGuid(), Quantity = 0 };
}
