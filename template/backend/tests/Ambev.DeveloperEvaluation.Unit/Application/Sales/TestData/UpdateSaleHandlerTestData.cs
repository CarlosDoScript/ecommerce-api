using Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;
using Bogus;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales.TestData;

public static class UpdateSaleHandlerTestData
{
    private static readonly Faker<UpdateSaleItemCommand> ItemFaker = new Faker<UpdateSaleItemCommand>()
        .RuleFor(i => i.ProductId, f => Guid.NewGuid())
        .RuleFor(i => i.ProductName, f => f.Commerce.ProductName())
        .RuleFor(i => i.UnitPrice, f => f.Random.Decimal(1m, 500m))
        .RuleFor(i => i.Quantity, f => f.Random.Int(1, 3))
        .RuleFor(i => i.IsCancelled, _ => false);

    private static readonly Faker<UpdateSaleCommand> Faker = new Faker<UpdateSaleCommand>()
        .RuleFor(s => s.Id, f => Guid.NewGuid())
        .RuleFor(s => s.Date, f => f.Date.Recent())
        .RuleFor(s => s.CustomerId, f => Guid.NewGuid())
        .RuleFor(s => s.CustomerName, f => f.Person.FullName)
        .RuleFor(s => s.BranchId, f => Guid.NewGuid())
        .RuleFor(s => s.BranchName, f => f.Company.CompanyName())
        .RuleFor(s => s.Items, f => ItemFaker.Generate(f.Random.Int(1, 3)));

    public static UpdateSaleCommand GenerateValidCommand() => Faker.Generate();

    public static UpdateSaleCommand GenerateCommandWithId(Guid id)
    {
        var cmd = Faker.Generate();
        return cmd with { Id = id };
    }
}
