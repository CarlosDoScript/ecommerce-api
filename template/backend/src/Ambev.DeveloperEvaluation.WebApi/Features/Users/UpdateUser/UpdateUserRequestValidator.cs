using Ambev.DeveloperEvaluation.Domain.Enums;
using FluentValidation;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Users.UpdateUser;

public class UpdateUserRequestValidator : AbstractValidator<UpdateUserRequest>
{
    public UpdateUserRequestValidator()
    {
        RuleFor(u => u.Username).NotEmpty().Length(3, 50);
        RuleFor(u => u.Email).NotEmpty().EmailAddress();
        RuleFor(u => u.Phone).Matches(@"^\+?[1-9]\d{1,14}$");
        RuleFor(u => u.Status).NotEqual(UserStatus.Unknown);
        RuleFor(u => u.Role).NotEqual(UserRole.None);
    }
}
