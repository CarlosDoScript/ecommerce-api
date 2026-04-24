using Ambev.DeveloperEvaluation.Application.Carts.UpdateCart;
using Bogus;

namespace Ambev.DeveloperEvaluation.Unit.Application.Carts.TestData;

/// <summary>
/// Provides methods for generating test data for the UpdateCartHandler.
/// </summary>
public static class UpdateCartHandlerTestData
{
    private static readonly Faker<UpdateCartCommand> _faker = new Faker<UpdateCartCommand>()
        .RuleFor(c => c.Id, f => Guid.NewGuid())
        .RuleFor(c => c.UserId, f => Guid.NewGuid())
        .RuleFor(c => c.Date, f => f.Date.Recent())
        .RuleFor(c => c.Products, f => new List<UpdateCartItemCommand>
        {
            new UpdateCartItemCommand { ProductId = Guid.NewGuid(), Quantity = f.Random.Int(1, 10) },
            new UpdateCartItemCommand { ProductId = Guid.NewGuid(), Quantity = f.Random.Int(1, 10) }
        });

    public static UpdateCartCommand GenerateValidCommand() => _faker.Generate();

    public static UpdateCartCommand GenerateCommandWithId(Guid id)
    {
        var cmd = _faker.Generate();
        return cmd with { Id = id };
    }
}
