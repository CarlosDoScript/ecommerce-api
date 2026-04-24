using Ambev.DeveloperEvaluation.Application.Carts.CreateCart;
using FluentValidation.TestHelper;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Carts.Validators;

/// <summary>
/// Contains unit tests for the <see cref="CreateCartCommandValidator"/> class.
/// </summary>
public class CreateCartCommandValidatorTests
{
    private readonly CreateCartCommandValidator _validator;

    public CreateCartCommandValidatorTests()
    {
        _validator = new CreateCartCommandValidator();
    }

    [Fact(DisplayName = "Valid cart command should pass all validation rules")]
    public void Given_ValidCommand_When_Validated_Then_ShouldNotHaveErrors()
    {
        // Arrange
        var command = new CreateCartCommand
        {
            UserId = Guid.NewGuid(),
            Date = DateTime.UtcNow,
            Products =
            [
                new CreateCartItemCommand { ProductId = Guid.NewGuid(), Quantity = 2 },
                new CreateCartItemCommand { ProductId = Guid.NewGuid(), Quantity = 5 }
            ]
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact(DisplayName = "Empty UserId should fail validation")]
    public void Given_EmptyUserId_When_Validated_Then_ShouldHaveError()
    {
        // Arrange
        var command = new CreateCartCommand
        {
            UserId = Guid.Empty,
            Date = DateTime.UtcNow,
            Products = [new CreateCartItemCommand { ProductId = Guid.NewGuid(), Quantity = 1 }]
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.UserId);
    }

    [Fact(DisplayName = "Default Date (empty) should fail validation")]
    public void Given_DefaultDate_When_Validated_Then_ShouldHaveError()
    {
        // Arrange
        var command = new CreateCartCommand
        {
            UserId = Guid.NewGuid(),
            Date = default,
            Products = [new CreateCartItemCommand { ProductId = Guid.NewGuid(), Quantity = 1 }]
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Date);
    }

    [Fact(DisplayName = "Empty products list should fail validation")]
    public void Given_EmptyProducts_When_Validated_Then_ShouldHaveError()
    {
        // Arrange
        var command = new CreateCartCommand
        {
            UserId = Guid.NewGuid(),
            Date = DateTime.UtcNow,
            Products = []
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Products);
    }

    [Fact(DisplayName = "Cart item with empty ProductId should fail validation")]
    public void Given_ItemWithEmptyProductId_When_Validated_Then_ShouldHaveError()
    {
        // Arrange
        var command = new CreateCartCommand
        {
            UserId = Guid.NewGuid(),
            Date = DateTime.UtcNow,
            Products = [new CreateCartItemCommand { ProductId = Guid.Empty, Quantity = 1 }]
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor("Products[0].ProductId");
    }

    [Theory(DisplayName = "Cart item with zero or negative quantity should fail validation")]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-10)]
    public void Given_ItemWithInvalidQuantity_When_Validated_Then_ShouldHaveError(int quantity)
    {
        // Arrange
        var command = new CreateCartCommand
        {
            UserId = Guid.NewGuid(),
            Date = DateTime.UtcNow,
            Products = [new CreateCartItemCommand { ProductId = Guid.NewGuid(), Quantity = quantity }]
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor("Products[0].Quantity");
    }
}
