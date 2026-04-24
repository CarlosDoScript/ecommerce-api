using Ambev.DeveloperEvaluation.Application.Carts.DeleteCart;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Carts;

/// <summary>
/// Contains unit tests for the <see cref="DeleteCartHandler"/> class.
/// </summary>
public class DeleteCartHandlerTests
{
    private readonly ICartRepository _cartRepository;
    private readonly DeleteCartHandler _handler;

    public DeleteCartHandlerTests()
    {
        _cartRepository = Substitute.For<ICartRepository>();
        _handler = new DeleteCartHandler(_cartRepository);
    }

    [Fact(DisplayName = "Given existing cart ID When deleting Then returns success")]
    public async Task Handle_ExistingCart_ReturnsSuccess()
    {
        // Given
        var cartId = Guid.NewGuid();
        _cartRepository.DeleteAsync(cartId, Arg.Any<CancellationToken>()).Returns(true);

        // When
        var result = await _handler.Handle(new DeleteCartCommand(cartId), CancellationToken.None);

        // Then
        result.Success.Should().BeTrue();
        await _cartRepository.Received(1).DeleteAsync(cartId, Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Given non-existing cart ID When deleting Then throws KeyNotFoundException")]
    public async Task Handle_NonExistingCart_ThrowsKeyNotFoundException()
    {
        // Given
        var cartId = Guid.NewGuid();
        _cartRepository.DeleteAsync(cartId, Arg.Any<CancellationToken>()).Returns(false);

        // When
        var act = () => _handler.Handle(new DeleteCartCommand(cartId), CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage($"*{cartId}*");
    }

    [Fact(DisplayName = "Given cart deletion When successful Then repository delete is called once")]
    public async Task Handle_SuccessfulDeletion_CallsRepositoryOnce()
    {
        // Given
        var cartId = Guid.NewGuid();
        _cartRepository.DeleteAsync(cartId, Arg.Any<CancellationToken>()).Returns(true);

        // When
        await _handler.Handle(new DeleteCartCommand(cartId), CancellationToken.None);

        // Then
        await _cartRepository.Received(1).DeleteAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());
    }
}
