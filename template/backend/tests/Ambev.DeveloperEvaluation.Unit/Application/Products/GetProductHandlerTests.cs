using Ambev.DeveloperEvaluation.Application.Products.GetProduct;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.ValueObjects;
using AutoMapper;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Products;

public class GetProductHandlerTests
{
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;
    private readonly GetProductHandler _handler;

    public GetProductHandlerTests()
    {
        _productRepository = Substitute.For<IProductRepository>();
        _mapper = Substitute.For<IMapper>();
        _handler = new GetProductHandler(_productRepository, _mapper);
    }

    [Fact(DisplayName = "Given existing product ID When getting product Then returns product")]
    public async Task Handle_ExistingProduct_ReturnsProduct()
    {
        // Given
        var productId = Guid.NewGuid();
        var product = new Product
        {
            Id = productId,
            Title = "Test Product",
            Price = 99.99m,
            Category = "electronics",
            Rating = new ProductRating { Rate = 4.5m, Count = 100 }
        };
        var result = new GetProductResult { Id = productId, Title = product.Title };

        _productRepository.GetByIdAsync(productId, Arg.Any<CancellationToken>()).Returns(product);
        _mapper.Map<GetProductResult>(product).Returns(result);

        // When
        var response = await _handler.Handle(new GetProductCommand(productId), CancellationToken.None);

        // Then
        response.Should().NotBeNull();
        response.Id.Should().Be(productId);
    }

    [Fact(DisplayName = "Given non-existing product ID When getting product Then throws KeyNotFoundException")]
    public async Task Handle_NonExistingProduct_ThrowsKeyNotFoundException()
    {
        // Given
        var productId = Guid.NewGuid();
        _productRepository.GetByIdAsync(productId, Arg.Any<CancellationToken>()).Returns((Product?)null);

        // When
        var act = () => _handler.Handle(new GetProductCommand(productId), CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<KeyNotFoundException>();
    }
}
