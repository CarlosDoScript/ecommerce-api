using Ambev.DeveloperEvaluation.Application.Carts.CreateCart;
using Bogus;

namespace Ambev.DeveloperEvaluation.Unit.Application.Carts.TestData;

public static class CreateCartHandlerTestData
{
    private static readonly Faker<CreateCartCommand> _faker = new Faker<CreateCartCommand>()
        .RuleFor(c => c.UserId, f => Guid.NewGuid())
        .RuleFor(c => c.Date, f => f.Date.Recent())
        .RuleFor(c => c.Products, f => new List<CreateCartItemCommand>
        {
            new CreateCartItemCommand
            {
                ProductId = Guid.NewGuid(),
                Quantity = f.Random.Int(1, 10)
            }
        });

    public static CreateCartCommand GenerateValidCommand() => _faker.Generate();
}
