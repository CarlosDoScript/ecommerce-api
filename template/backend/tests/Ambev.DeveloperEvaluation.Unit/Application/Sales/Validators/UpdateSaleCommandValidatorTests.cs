using Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;
using Ambev.DeveloperEvaluation.Unit.Application.Sales.TestData;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales.Validators;

public class UpdateSaleCommandValidatorTests
{
    private readonly UpdateSaleCommandValidator _validator = new();

    [Fact(DisplayName = "Given valid command When validating Then passes validation")]
    public async Task Validate_ValidCommand_PassesValidation()
    {
        // Given
        var command = UpdateSaleHandlerTestData.GenerateValidCommand();

        // When
        var result = await _validator.ValidateAsync(command);

        // Then
        result.IsValid.Should().BeTrue();
    }

    [Fact(DisplayName = "Given empty Id When validating Then fails validation")]
    public void Validate_EmptyId_FailsValidation()
    {
        // Given
        var command = UpdateSaleHandlerTestData.GenerateValidCommand() with { Id = Guid.Empty };

        // When
        var result = _validator.Validate(command);

        // Then
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Id");
    }

    [Fact(DisplayName = "Given empty CustomerId When validating Then fails validation")]
    public void Validate_EmptyCustomerId_FailsValidation()
    {
        // Given
        var command = UpdateSaleHandlerTestData.GenerateValidCommand() with { CustomerId = Guid.Empty };

        // When
        var result = _validator.Validate(command);

        // Then
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "CustomerId");
    }

    [Fact(DisplayName = "Given empty CustomerName When validating Then fails validation")]
    public void Validate_EmptyCustomerName_FailsValidation()
    {
        // Given
        var command = UpdateSaleHandlerTestData.GenerateValidCommand() with { CustomerName = string.Empty };

        // When
        var result = _validator.Validate(command);

        // Then
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "CustomerName");
    }

    [Fact(DisplayName = "Given empty BranchId When validating Then fails validation")]
    public void Validate_EmptyBranchId_FailsValidation()
    {
        // Given
        var command = UpdateSaleHandlerTestData.GenerateValidCommand() with { BranchId = Guid.Empty };

        // When
        var result = _validator.Validate(command);

        // Then
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "BranchId");
    }

    [Fact(DisplayName = "Given empty Items list When validating Then fails validation")]
    public void Validate_EmptyItemsList_FailsValidation()
    {
        // Given
        var command = UpdateSaleHandlerTestData.GenerateValidCommand() with { Items = [] };

        // When
        var result = _validator.Validate(command);

        // Then
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Items");
    }

    [Theory(DisplayName = "Given item quantity <= 20 When validating Then passes validation")]
    [InlineData(1)]
    [InlineData(4)]
    [InlineData(10)]
    [InlineData(20)]
    public void Validate_ItemQuantityAtOrBelowLimit_PassesValidation(int quantity)
    {
        // Given
        var command = UpdateSaleHandlerTestData.GenerateValidCommand() with
        {
            Items =
            [
                new UpdateSaleItemCommand
                {
                    ProductId = Guid.NewGuid(),
                    ProductName = "Valid Product",
                    UnitPrice = 10m,
                    Quantity = quantity,
                    IsCancelled = false
                }
            ]
        };

        // When
        var result = _validator.Validate(command);

        // Then
        result.IsValid.Should().BeTrue();
    }

    [Theory(DisplayName = "Given item quantity > 20 When validating Then fails validation")]
    [InlineData(21)]
    [InlineData(50)]
    [InlineData(100)]
    public void Validate_ItemQuantityAboveLimit_FailsValidation(int quantity)
    {
        // Given
        var command = UpdateSaleHandlerTestData.GenerateValidCommand() with
        {
            Items =
            [
                new UpdateSaleItemCommand
                {
                    ProductId = Guid.NewGuid(),
                    ProductName = "Test Product",
                    UnitPrice = 10m,
                    Quantity = quantity,
                    IsCancelled = false
                }
            ]
        };

        // When
        var result = _validator.Validate(command);

        // Then
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName.Contains("Quantity"));
    }

    [Theory(DisplayName = "Given item quantity <= 0 When validating Then fails validation")]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validate_ItemQuantityZeroOrNegative_FailsValidation(int quantity)
    {
        // Given
        var command = UpdateSaleHandlerTestData.GenerateValidCommand() with
        {
            Items =
            [
                new UpdateSaleItemCommand
                {
                    ProductId = Guid.NewGuid(),
                    ProductName = "Test Product",
                    UnitPrice = 10m,
                    Quantity = quantity,
                    IsCancelled = false
                }
            ]
        };

        // When
        var result = _validator.Validate(command);

        // Then
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName.Contains("Quantity"));
    }

    [Fact(DisplayName = "Given cancelled item with quantity > 20 When validating Then fails validation")]
    public void Validate_CancelledItemWithQuantityAboveLimit_FailsValidation()
    {
        // Given — even cancelled items must respect the validator's quantity constraint
        var command = UpdateSaleHandlerTestData.GenerateValidCommand() with
        {
            Items =
            [
                new UpdateSaleItemCommand
                {
                    ProductId = Guid.NewGuid(),
                    ProductName = "Test Product",
                    UnitPrice = 10m,
                    Quantity = 25,
                    IsCancelled = true
                }
            ]
        };

        // When
        var result = _validator.Validate(command);

        // Then
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName.Contains("Quantity"));
    }
}
