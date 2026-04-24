using Ambev.DeveloperEvaluation.Application.Users.UpdateUser;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Domain.ValueObjects;
using Bogus;

namespace Ambev.DeveloperEvaluation.Unit.Application.Users.TestData;

public static class UpdateUserHandlerTestData
{
    private static readonly Faker<UpdateUserCommand> Faker = new Faker<UpdateUserCommand>()
        .RuleFor(u => u.Id, f => Guid.NewGuid())
        .RuleFor(u => u.Username, f => f.Internet.UserName())
        .RuleFor(u => u.Email, f => f.Internet.Email())
        .RuleFor(u => u.Phone, f => "+5511987654321")
        .RuleFor(u => u.Status, f => UserStatus.Active)
        .RuleFor(u => u.Role, f => UserRole.Customer)
        .RuleFor(u => u.Name, f => new UserName { Firstname = f.Name.FirstName(), Lastname = f.Name.LastName() })
        .RuleFor(u => u.Address, f => new UserAddress
        {
            City = f.Address.City(),
            Street = f.Address.StreetName(),
            Number = f.Random.Int(1, 999),
            Zipcode = f.Address.ZipCode(),
            Lat = f.Address.Latitude().ToString(),
            Long = f.Address.Longitude().ToString()
        });

    public static UpdateUserCommand GenerateValidCommand() => Faker.Generate();

    public static UpdateUserCommand GenerateCommandWithId(Guid id)
    {
        var cmd = Faker.Generate();
        cmd.Id = id;
        return cmd;
    }
}
