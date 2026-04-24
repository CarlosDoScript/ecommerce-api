using Ambev.DeveloperEvaluation.Application.Products.CreateProduct;
using Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;
using FluentValidation.TestHelper;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Products.Validators;

/// <summary>
/// Contains unit tests for the <see cref="CreateProductCommandValidator"/> class.
/// Tests cover all validation rules for product creation.
/// </summary>
public class CreateProductCommandValidatorTests
{
    private readonly CreateProductCommandValidator _validator;

    public CreateProductCommandValidatorTests()
    {
        _validator = new CreateProductCommandValidator();
    }

    [Fact(DisplayName = "Valid product command should pass all validation rules")]
    public void Given_ValidCommand_When_Validated_Then_ShouldNotHaveErrors()
    {
        // Arrange
        var command = new CreateProductCommand
        {
            Title = ProductTestData.GenerateValidTitle(),
            Price = ProductTestData.GenerateValidPrice(),
            Description = "A great product description",
            Category = ProductTestData.GenerateValidCategory(),
            Image = "https://example.com/image.jpg",
            Rating = new CreateProductRatingCommand { Rate = 4.5m, Count = 100 }
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory(DisplayName = "Empty or null title should fail validation")]
    [InlineData("")]
    [InlineData("   ")]
    public void Given_InvalidTitle_When_Validated_Then_ShouldHaveError(string title)
    {
        // Arrange
        var command = new CreateProductCommand { Title = title, Price = 10m, Description = "desc", Category = "cat", Rating = new() };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Title);
    }

    [Fact(DisplayName = "Title exceeding 200 characters should fail validation")]
    public void Given_TitleExceedingMaxLength_When_Validated_Then_ShouldHaveError()
    {
        // Arrange
        var command = new CreateProductCommand
        {
            Title = ProductTestData.GenerateLongTitle(),
            Price = 10m, Description = "desc", Category = "cat", Rating = new()
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Title);
    }

    [Theory(DisplayName = "Price of zero or negative should fail validation")]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-99.99)]
    public void Given_InvalidPrice_When_Validated_Then_ShouldHaveError(double price)
    {
        // Arrange
        var command = new CreateProductCommand
        {
            Title = "Valid Title", Price = (decimal)price, Description = "desc", Category = "cat", Rating = new()
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Price);
    }

    [Theory(DisplayName = "Empty or null category should fail validation")]
    [InlineData("")]
    [InlineData("   ")]
    public void Given_InvalidCategory_When_Validated_Then_ShouldHaveError(string category)
    {
        // Arrange
        var command = new CreateProductCommand
        {
            Title = "Valid Title", Price = 10m, Description = "desc", Category = category, Rating = new()
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Category);
    }

    [Fact(DisplayName = "Category exceeding 100 characters should fail validation")]
    public void Given_CategoryExceedingMaxLength_When_Validated_Then_ShouldHaveError()
    {
        // Arrange
        var command = new CreateProductCommand
        {
            Title = "Valid Title", Price = 10m, Description = "desc",
            Category = ProductTestData.GenerateLongCategory(), Rating = new()
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Category);
    }

    [Theory(DisplayName = "Empty description should fail validation")]
    [InlineData("")]
    [InlineData("   ")]
    public void Given_EmptyDescription_When_Validated_Then_ShouldHaveError(string description)
    {
        // Arrange
        var command = new CreateProductCommand
        {
            Title = "Valid Title", Price = 10m, Description = description, Category = "cat", Rating = new()
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Description);
    }

    [Fact(DisplayName = "Rating rate above 5 should fail validation")]
    public void Given_RatingRateAboveMax_When_Validated_Then_ShouldHaveError()
    {
        // Arrange
        var command = new CreateProductCommand
        {
            Title = "Valid Title", Price = 10m, Description = "desc", Category = "cat",
            Rating = new CreateProductRatingCommand { Rate = ProductTestData.GenerateInvalidRate(), Count = 10 }
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Rating.Rate);
    }

    [Fact(DisplayName = "Negative rating count should fail validation")]
    public void Given_NegativeRatingCount_When_Validated_Then_ShouldHaveError()
    {
        // Arrange
        var command = new CreateProductCommand
        {
            Title = "Valid Title", Price = 10m, Description = "desc", Category = "cat",
            Rating = new CreateProductRatingCommand { Rate = 4m, Count = ProductTestData.GenerateInvalidCount() }
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Rating.Count);
    }
}
