using Ambev.DeveloperEvaluation.Application.Carts.UpdateCart;
using FluentValidation.TestHelper;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Carts.Validators;

/// <summary>
/// Contains unit tests for the <see cref="UpdateCartCommandValidator"/> class.
/// </summary>
public class UpdateCartCommandValidatorTests
{
    private readonly UpdateCartCommandValidator _validator;

    public UpdateCartCommandValidatorTests()
    {
        _validator = new UpdateCartCommandValidator();
    }

    [Fact(DisplayName = "Valid update cart command should pass all validation rules")]
    public void Given_ValidCommand_When_Validated_Then_ShouldNotHaveErrors()
    {
        // Arrange
        var command = new UpdateCartCommand
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            Date = DateTime.UtcNow,
            Products =
            [
                new UpdateCartItemCommand { ProductId = Guid.NewGuid(), Quantity = 3 },
                new UpdateCartItemCommand { ProductId = Guid.NewGuid(), Quantity = 1 }
            ]
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact(DisplayName = "Empty Id should fail validation")]
    public void Given_EmptyId_When_Validated_Then_ShouldHaveError()
    {
        // Arrange
        var command = new UpdateCartCommand
        {
            Id = Guid.Empty,
            UserId = Guid.NewGuid(),
            Date = DateTime.UtcNow,
            Products = [new UpdateCartItemCommand { ProductId = Guid.NewGuid(), Quantity = 1 }]
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }

    [Fact(DisplayName = "Empty UserId should fail validation")]
    public void Given_EmptyUserId_When_Validated_Then_ShouldHaveError()
    {
        // Arrange
        var command = new UpdateCartCommand
        {
            Id = Guid.NewGuid(),
            UserId = Guid.Empty,
            Date = DateTime.UtcNow,
            Products = [new UpdateCartItemCommand { ProductId = Guid.NewGuid(), Quantity = 1 }]
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
        var command = new UpdateCartCommand
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            Date = default,
            Products = [new UpdateCartItemCommand { ProductId = Guid.NewGuid(), Quantity = 1 }]
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
        var command = new UpdateCartCommand
        {
            Id = Guid.NewGuid(),
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
        var command = new UpdateCartCommand
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            Date = DateTime.UtcNow,
            Products = [new UpdateCartItemCommand { ProductId = Guid.Empty, Quantity = 1 }]
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
        var command = new UpdateCartCommand
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            Date = DateTime.UtcNow,
            Products = [new UpdateCartItemCommand { ProductId = Guid.NewGuid(), Quantity = quantity }]
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor("Products[0].Quantity");
    }
}
