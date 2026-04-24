using Ambev.DeveloperEvaluation.Application.Products.ListProducts;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.ValueObjects;
using AutoMapper;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Products;

/// <summary>
/// Contains unit tests for the <see cref="ListProductsHandler"/> class.
/// </summary>
public class ListProductsHandlerTests
{
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;
    private readonly ListProductsHandler _handler;

    public ListProductsHandlerTests()
    {
        _productRepository = Substitute.For<IProductRepository>();
        _mapper = Substitute.For<IMapper>();
        _handler = new ListProductsHandler(_productRepository, _mapper);
    }

    [Fact(DisplayName = "Given valid pagination parameters When listing products Then returns paginated result")]
    public async Task Handle_ValidPagination_ReturnsPaginatedResult()
    {
        // Given
        var command = new ListProductsCommand { Page = 1, Size = 10 };
        var products = GenerateProducts(3);
        var mappedItems = products.Select(p => new ProductListItem { Id = p.Id, Title = p.Title }).ToList();

        _productRepository.GetPagedAsync(1, 10, null, null, null, null, null, Arg.Any<CancellationToken>())
            .Returns((products, 3));
        _mapper.Map<IEnumerable<ProductListItem>>(Arg.Any<IEnumerable<Product>>()).Returns(mappedItems);

        // When
        var result = await _handler.Handle(command, CancellationToken.None);

        // Then
        result.Should().NotBeNull();
        result.Data.Should().HaveCount(3);
        result.TotalItems.Should().Be(3);
        result.CurrentPage.Should().Be(1);
        result.TotalPages.Should().Be(1);
    }

    [Fact(DisplayName = "Given multiple pages When requesting page 2 Then returns correct pagination metadata")]
    public async Task Handle_MultiplePagesRequest_ReturnsCorrectMetadata()
    {
        // Given
        var command = new ListProductsCommand { Page = 2, Size = 5 };
        var products = GenerateProducts(5);

        _productRepository.GetPagedAsync(2, 5, null, null, null, null, null, Arg.Any<CancellationToken>())
            .Returns((products, 12));
        _mapper.Map<IEnumerable<ProductListItem>>(Arg.Any<IEnumerable<Product>>())
            .Returns(products.Select(p => new ProductListItem { Id = p.Id }).ToList());

        // When
        var result = await _handler.Handle(command, CancellationToken.None);

        // Then
        result.TotalItems.Should().Be(12);
        result.CurrentPage.Should().Be(2);
        result.TotalPages.Should().Be(3); // ceil(12/5) = 3
    }

    [Fact(DisplayName = "Given empty database When listing products Then returns empty result")]
    public async Task Handle_EmptyRepository_ReturnsEmptyResult()
    {
        // Given
        var command = new ListProductsCommand { Page = 1, Size = 10 };

        _productRepository.GetPagedAsync(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<string>(),
                Arg.Any<string>(), Arg.Any<string>(), Arg.Any<decimal?>(), Arg.Any<decimal?>(),
                Arg.Any<CancellationToken>())
            .Returns((Enumerable.Empty<Product>(), 0));
        _mapper.Map<IEnumerable<ProductListItem>>(Arg.Any<IEnumerable<Product>>())
            .Returns(Enumerable.Empty<ProductListItem>());

        // When
        var result = await _handler.Handle(command, CancellationToken.None);

        // Then
        result.Data.Should().BeEmpty();
        result.TotalItems.Should().Be(0);
        result.TotalPages.Should().Be(0);
    }

    [Fact(DisplayName = "Given filter parameters When listing products Then passes filters to repository")]
    public async Task Handle_WithFilters_PassesFiltersToRepository()
    {
        // Given
        var command = new ListProductsCommand
        {
            Page = 1, Size = 10,
            Title = "backpack*",
            Category = "bags",
            MinPrice = 50m,
            MaxPrice = 200m
        };

        _productRepository.GetPagedAsync(1, 10, null, "backpack*", "bags", 50m, 200m, Arg.Any<CancellationToken>())
            .Returns((Enumerable.Empty<Product>(), 0));
        _mapper.Map<IEnumerable<ProductListItem>>(Arg.Any<IEnumerable<Product>>())
            .Returns(Enumerable.Empty<ProductListItem>());

        // When
        await _handler.Handle(command, CancellationToken.None);

        // Then
        await _productRepository.Received(1).GetPagedAsync(
            1, 10, null, "backpack*", "bags", 50m, 200m, Arg.Any<CancellationToken>());
    }

    private static List<Product> GenerateProducts(int count) =>
        Enumerable.Range(0, count).Select(_ => new Product
        {
            Id = Guid.NewGuid(),
            Title = $"Product {Guid.NewGuid()}",
            Price = 99m,
            Category = "test",
            Rating = new ProductRating { Rate = 4m, Count = 10 }
        }).ToList();
}
