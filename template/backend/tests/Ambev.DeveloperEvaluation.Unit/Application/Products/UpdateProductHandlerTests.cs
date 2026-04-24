using Ambev.DeveloperEvaluation.Application.Products.UpdateProduct;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.ValueObjects;
using Ambev.DeveloperEvaluation.Unit.Application.Products.TestData;
using AutoMapper;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Products;

/// <summary>
/// Contains unit tests for the <see cref="UpdateProductHandler"/> class.
/// </summary>
public class UpdateProductHandlerTests
{
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;
    private readonly UpdateProductHandler _handler;

    public UpdateProductHandlerTests()
    {
        _productRepository = Substitute.For<IProductRepository>();
        _mapper = Substitute.For<IMapper>();
        _handler = new UpdateProductHandler(_productRepository, _mapper);
    }

    [Fact(DisplayName = "Given valid update data When product exists Then returns updated product")]
    public async Task Handle_ValidCommand_ExistingProduct_ReturnsUpdatedResult()
    {
        // Given
        var command = UpdateProductHandlerTestData.GenerateValidCommand();
        var existingProduct = new Product
        {
            Id = command.Id,
            Title = "Old Title",
            Price = 10m,
            Description = "Old description",
            Category = "old-category",
            Image = "http://old.img",
            Rating = new ProductRating { Rate = 1m, Count = 5 }
        };
        var updatedResult = new UpdateProductResult
        {
            Id = command.Id,
            Title = command.Title,
            Price = command.Price
        };

        _productRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>()).Returns(existingProduct);
        _productRepository.UpdateAsync(Arg.Any<Product>(), Arg.Any<CancellationToken>()).Returns(existingProduct);
        _mapper.Map<ProductRating>(Arg.Any<UpdateProductRatingCommand>()).Returns(new ProductRating());
        _mapper.Map<UpdateProductResult>(Arg.Any<Product>()).Returns(updatedResult);

        // When
        var result = await _handler.Handle(command, CancellationToken.None);

        // Then
        result.Should().NotBeNull();
        result.Id.Should().Be(command.Id);
        await _productRepository.Received(1).UpdateAsync(Arg.Any<Product>(), Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Given non-existing product ID When updating Then throws KeyNotFoundException")]
    public async Task Handle_NonExistingProduct_ThrowsKeyNotFoundException()
    {
        // Given
        var command = UpdateProductHandlerTestData.GenerateValidCommand();
        _productRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>()).Returns((Product?)null);

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage($"*{command.Id}*");
    }

    [Fact(DisplayName = "Given invalid data When updating Then throws ValidationException")]
    public async Task Handle_InvalidCommand_ThrowsValidationException()
    {
        // Given
        var command = new UpdateProductCommand(); // Price = 0, fails validation

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<FluentValidation.ValidationException>();
    }

    [Fact(DisplayName = "Given valid command When updating Then product fields are updated")]
    public async Task Handle_ValidCommand_UpdatesProductFields()
    {
        // Given
        var command = UpdateProductHandlerTestData.GenerateValidCommand();
        var existingProduct = new Product
        {
            Id = command.Id,
            Title = "Original",
            Price = 1m,
            Description = "Original desc",
            Category = "original",
            Rating = new ProductRating { Rate = 1m, Count = 1 }
        };

        _productRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>()).Returns(existingProduct);
        _productRepository.UpdateAsync(Arg.Any<Product>(), Arg.Any<CancellationToken>())
            .Returns(ci => ci.Arg<Product>());
        _mapper.Map<UpdateProductResult>(Arg.Any<Product>()).Returns(new UpdateProductResult());

        // When
        await _handler.Handle(command, CancellationToken.None);

        // Then
        await _productRepository.Received(1).UpdateAsync(
            Arg.Is<Product>(p =>
                p.Title == command.Title &&
                p.Price == command.Price &&
                p.Category == command.Category &&
                p.UpdatedAt != null),
            Arg.Any<CancellationToken>());
    }
}
