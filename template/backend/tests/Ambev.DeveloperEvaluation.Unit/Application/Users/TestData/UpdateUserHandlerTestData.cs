using Ambev.DeveloperEvaluation.Application.Users.UpdateUser;
using Ambev.DeveloperEvaluation.Domain.Enums;
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
        .RuleFor(u => u.Role, f => UserRole.Customer);

    public static UpdateUserCommand GenerateValidCommand() => Faker.Generate();

    public static UpdateUserCommand GenerateCommandWithId(Guid id)
    {
        var cmd = Faker.Generate();
        cmd.Id = id;
        return cmd;
    }
}
