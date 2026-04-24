using Ambev.DeveloperEvaluation.Application.Products.DeleteProduct;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Products;

public class DeleteProductHandlerTests
{
    private readonly IProductRepository _productRepository;
    private readonly DeleteProductHandler _handler;

    public DeleteProductHandlerTests()
    {
        _productRepository = Substitute.For<IProductRepository>();
        _handler = new DeleteProductHandler(_productRepository);
    }

    [Fact(DisplayName = "Given existing product ID When deleting Then returns success")]
    public async Task Handle_ExistingProduct_ReturnsSuccess()
    {
        // Given
        var productId = Guid.NewGuid();
        _productRepository.DeleteAsync(productId, Arg.Any<CancellationToken>()).Returns(true);

        // When
        var result = await _handler.Handle(new DeleteProductCommand(productId), CancellationToken.None);

        // Then
        result.Success.Should().BeTrue();
        await _productRepository.Received(1).DeleteAsync(productId, Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Given non-existing product ID When deleting Then throws KeyNotFoundException")]
    public async Task Handle_NonExistingProduct_ThrowsKeyNotFoundException()
    {
        // Given
        var productId = Guid.NewGuid();
        _productRepository.DeleteAsync(productId, Arg.Any<CancellationToken>()).Returns(false);

        // When
        var act = () => _handler.Handle(new DeleteProductCommand(productId), CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<KeyNotFoundException>();
    }
}
