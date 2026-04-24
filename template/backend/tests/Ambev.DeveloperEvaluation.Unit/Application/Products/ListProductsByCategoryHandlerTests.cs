using Ambev.DeveloperEvaluation.Application.Products.ListProductsByCategory;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.ValueObjects;
using AutoMapper;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Products;

/// <summary>
/// Contains unit tests for the <see cref="ListProductsByCategoryHandler"/> class.
/// </summary>
public class ListProductsByCategoryHandlerTests
{
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;
    private readonly ListProductsByCategoryHandler _handler;

    public ListProductsByCategoryHandlerTests()
    {
        _productRepository = Substitute.For<IProductRepository>();
        _mapper = Substitute.For<IMapper>();
        _handler = new ListProductsByCategoryHandler(_productRepository, _mapper);
    }

    [Fact(DisplayName = "Given valid category When listing Then returns products of that category")]
    public async Task Handle_ValidCategory_ReturnsProductsInCategory()
    {
        // Given
        const string category = "electronics";
        var command = new ListProductsByCategoryCommand { Category = category, Page = 1, Size = 10 };
        var products = new List<Product>
        {
            new() { Id = Guid.NewGuid(), Title = "Phone", Category = category, Price = 500m, Rating = new ProductRating() },
            new() { Id = Guid.NewGuid(), Title = "Laptop", Category = category, Price = 1200m, Rating = new ProductRating() }
        };
        var mappedItems = products.Select(p => new ProductByCategoryItem { Id = p.Id, Category = category }).ToList();

        _productRepository.GetByCategoryPagedAsync(category, 1, 10, null, Arg.Any<CancellationToken>())
            .Returns((products, 2));
        _mapper.Map<IEnumerable<ProductByCategoryItem>>(Arg.Any<IEnumerable<Product>>()).Returns(mappedItems);

        // When
        var result = await _handler.Handle(command, CancellationToken.None);

        // Then
        result.Should().NotBeNull();
        result.Data.Should().HaveCount(2);
        result.TotalItems.Should().Be(2);
        result.CurrentPage.Should().Be(1);
    }

    [Fact(DisplayName = "Given empty category string When listing Then throws ValidationException")]
    public async Task Handle_EmptyCategory_ThrowsValidationException()
    {
        // Given
        var command = new ListProductsByCategoryCommand { Category = "", Page = 1, Size = 10 };

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<FluentValidation.ValidationException>();
    }

    [Fact(DisplayName = "Given category with no products When listing Then returns empty paginated result")]
    public async Task Handle_CategoryWithNoProducts_ReturnsEmptyResult()
    {
        // Given
        var command = new ListProductsByCategoryCommand { Category = "nonexistent", Page = 1, Size = 10 };

        _productRepository.GetByCategoryPagedAsync("nonexistent", 1, 10, null, Arg.Any<CancellationToken>())
            .Returns((Enumerable.Empty<Product>(), 0));
        _mapper.Map<IEnumerable<ProductByCategoryItem>>(Arg.Any<IEnumerable<Product>>())
            .Returns(Enumerable.Empty<ProductByCategoryItem>());

        // When
        var result = await _handler.Handle(command, CancellationToken.None);

        // Then
        result.Data.Should().BeEmpty();
        result.TotalItems.Should().Be(0);
        result.TotalPages.Should().Be(0);
    }

    [Fact(DisplayName = "Given ordering parameter When listing by category Then passes order to repository")]
    public async Task Handle_WithOrderParameter_PassesOrderToRepository()
    {
        // Given
        var command = new ListProductsByCategoryCommand { Category = "electronics", Page = 1, Size = 5, Order = "price desc" };

        _productRepository.GetByCategoryPagedAsync("electronics", 1, 5, "price desc", Arg.Any<CancellationToken>())
            .Returns((Enumerable.Empty<Product>(), 0));
        _mapper.Map<IEnumerable<ProductByCategoryItem>>(Arg.Any<IEnumerable<Product>>())
            .Returns(Enumerable.Empty<ProductByCategoryItem>());

        // When
        await _handler.Handle(command, CancellationToken.None);

        // Then
        await _productRepository.Received(1)
            .GetByCategoryPagedAsync("electronics", 1, 5, "price desc", Arg.Any<CancellationToken>());
    }
}
