using Ambev.DeveloperEvaluation.Application.Users.CreateUser;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Unit.Application.Users.TestData;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Users.Validators;

public class CreateUserCommandValidatorTests
{
    private readonly CreateUserCommandValidator _validator = new();

    [Fact(DisplayName = "Given valid command When validating Then passes validation")]
    public void Validate_ValidCommand_PassesValidation()
    {
        // Given
        var command = CreateUserHandlerTestData.GenerateValidCommand();

        // When
        var result = _validator.Validate(command);

        // Then
        result.IsValid.Should().BeTrue();
    }

    [Fact(DisplayName = "Given empty username When validating Then fails validation")]
    public void Validate_EmptyUsername_FailsValidation()
    {
        // Given
        var command = CreateUserHandlerTestData.GenerateValidCommand();
        command.Username = string.Empty;

        // When
        var result = _validator.Validate(command);

        // Then
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Username");
    }

    [Fact(DisplayName = "Given invalid email When validating Then fails validation")]
    public void Validate_InvalidEmail_FailsValidation()
    {
        // Given
        var command = CreateUserHandlerTestData.GenerateValidCommand();
        command.Email = "not-an-email";

        // When
        var result = _validator.Validate(command);

        // Then
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Email");
    }

    [Fact(DisplayName = "Given Unknown status When validating Then fails validation")]
    public void Validate_UnknownStatus_FailsValidation()
    {
        // Given
        var command = CreateUserHandlerTestData.GenerateValidCommand();
        command.Status = UserStatus.Unknown;

        // When
        var result = _validator.Validate(command);

        // Then
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Status");
    }

    [Fact(DisplayName = "Given None role When validating Then fails validation")]
    public void Validate_NoneRole_FailsValidation()
    {
        // Given
        var command = CreateUserHandlerTestData.GenerateValidCommand();
        command.Role = UserRole.None;

        // When
        var result = _validator.Validate(command);

        // Then
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Role");
    }
}
