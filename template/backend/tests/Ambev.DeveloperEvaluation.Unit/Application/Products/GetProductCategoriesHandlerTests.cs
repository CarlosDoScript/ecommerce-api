using Ambev.DeveloperEvaluation.Application.Products.GetProductCategories;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Products;

/// <summary>
/// Contains unit tests for the <see cref="GetProductCategoriesHandler"/> class.
/// </summary>
public class GetProductCategoriesHandlerTests
{
    private readonly IProductRepository _productRepository;
    private readonly GetProductCategoriesHandler _handler;

    public GetProductCategoriesHandlerTests()
    {
        _productRepository = Substitute.For<IProductRepository>();
        _handler = new GetProductCategoriesHandler(_productRepository);
    }

    [Fact(DisplayName = "When categories exist Then returns all distinct categories")]
    public async Task Handle_WithCategories_ReturnsAllCategories()
    {
        // Given
        var categories = new[] { "electronics", "clothing", "jewelery", "men's clothing" };
        _productRepository.GetCategoriesAsync(Arg.Any<CancellationToken>()).Returns(categories);

        // When
        var result = await _handler.Handle(new GetProductCategoriesQuery(), CancellationToken.None);

        // Then
        result.Should().NotBeNull();
        result.Categories.Should().BeEquivalentTo(categories);
        result.Categories.Should().HaveCount(4);
    }

    [Fact(DisplayName = "When no products exist Then returns empty categories list")]
    public async Task Handle_NoCategoriesInRepository_ReturnsEmptyList()
    {
        // Given
        _productRepository.GetCategoriesAsync(Arg.Any<CancellationToken>())
            .Returns(Enumerable.Empty<string>());

        // When
        var result = await _handler.Handle(new GetProductCategoriesQuery(), CancellationToken.None);

        // Then
        result.Categories.Should().BeEmpty();
    }

    [Fact(DisplayName = "When categories are requested Then repository is called exactly once")]
    public async Task Handle_Always_CallsRepositoryOnce()
    {
        // Given
        _productRepository.GetCategoriesAsync(Arg.Any<CancellationToken>())
            .Returns(new[] { "electronics" });

        // When
        await _handler.Handle(new GetProductCategoriesQuery(), CancellationToken.None);

        // Then
        await _productRepository.Received(1).GetCategoriesAsync(Arg.Any<CancellationToken>());
    }
}
