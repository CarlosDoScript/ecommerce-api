using Ambev.DeveloperEvaluation.Application.Carts.GetCart;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Carts;

/// <summary>
/// Contains unit tests for the <see cref="GetCartHandler"/> class.
/// </summary>
public class GetCartHandlerTests
{
    private readonly ICartRepository _cartRepository;
    private readonly IMapper _mapper;
    private readonly GetCartHandler _handler;

    public GetCartHandlerTests()
    {
        _cartRepository = Substitute.For<ICartRepository>();
        _mapper = Substitute.For<IMapper>();
        _handler = new GetCartHandler(_cartRepository, _mapper);
    }

    [Fact(DisplayName = "Given existing cart ID When getting cart Then returns cart with products")]
    public async Task Handle_ExistingCart_ReturnsCartWithProducts()
    {
        // Given
        var cartId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var cart = new Cart
        {
            Id = cartId,
            UserId = userId,
            Date = DateTime.UtcNow,
            Products = new List<CartItem>
            {
                new CartItem { ProductId = Guid.NewGuid(), Quantity = 2 },
                new CartItem { ProductId = Guid.NewGuid(), Quantity = 1 }
            }
        };
        var result = new GetCartResult
        {
            Id = cartId,
            UserId = userId,
            Products = cart.Products.Select(p => new GetCartItemResult { ProductId = p.ProductId, Quantity = p.Quantity }).ToList()
        };

        _cartRepository.GetByIdAsync(cartId, Arg.Any<CancellationToken>()).Returns(cart);
        _mapper.Map<GetCartResult>(cart).Returns(result);

        // When
        var response = await _handler.Handle(new GetCartCommand(cartId), CancellationToken.None);

        // Then
        response.Should().NotBeNull();
        response.Id.Should().Be(cartId);
        response.UserId.Should().Be(userId);
        response.Products.Should().HaveCount(2);
    }

    [Fact(DisplayName = "Given non-existing cart ID When getting Then throws KeyNotFoundException")]
    public async Task Handle_NonExistingCart_ThrowsKeyNotFoundException()
    {
        // Given
        var cartId = Guid.NewGuid();
        _cartRepository.GetByIdAsync(cartId, Arg.Any<CancellationToken>()).Returns((Cart?)null);

        // When
        var act = () => _handler.Handle(new GetCartCommand(cartId), CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage($"*{cartId}*");
    }

    [Fact(DisplayName = "Given existing cart When getting Then repository is called with correct ID")]
    public async Task Handle_ExistingCart_CallsRepositoryWithCorrectId()
    {
        // Given
        var cartId = Guid.NewGuid();
        var cart = new Cart { Id = cartId, UserId = Guid.NewGuid(), Products = [] };
        _cartRepository.GetByIdAsync(cartId, Arg.Any<CancellationToken>()).Returns(cart);
        _mapper.Map<GetCartResult>(Arg.Any<Cart>()).Returns(new GetCartResult());

        // When
        await _handler.Handle(new GetCartCommand(cartId), CancellationToken.None);

        // Then
        await _cartRepository.Received(1).GetByIdAsync(cartId, Arg.Any<CancellationToken>());
    }
}
