using Ambev.DeveloperEvaluation.Application.Carts.CreateCart;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Unit.Application.Carts.TestData;
using AutoMapper;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Carts;

public class CreateCartHandlerTests
{
    private readonly ICartRepository _cartRepository;
    private readonly IMapper _mapper;
    private readonly CreateCartHandler _handler;

    public CreateCartHandlerTests()
    {
        _cartRepository = Substitute.For<ICartRepository>();
        _mapper = Substitute.For<IMapper>();
        _handler = new CreateCartHandler(_cartRepository, _mapper);
    }

    [Fact(DisplayName = "Given valid cart data When creating cart Then returns success response")]
    public async Task Handle_ValidRequest_ReturnsSuccessResponse()
    {
        // Given
        var command = CreateCartHandlerTestData.GenerateValidCommand();
        var cart = new Cart
        {
            Id = Guid.NewGuid(),
            UserId = command.UserId,
            Date = command.Date,
            Products = command.Products.Select(p => new CartItem
            {
                ProductId = p.ProductId,
                Quantity = p.Quantity
            }).ToList()
        };

        var result = new CreateCartResult { Id = cart.Id, UserId = cart.UserId, Date = cart.Date };

        _mapper.Map<Cart>(command).Returns(cart);
        _mapper.Map<CreateCartResult>(cart).Returns(result);
        _cartRepository.CreateAsync(Arg.Any<Cart>(), Arg.Any<CancellationToken>()).Returns(cart);

        // When
        var createResult = await _handler.Handle(command, CancellationToken.None);

        // Then
        createResult.Should().NotBeNull();
        createResult.Id.Should().Be(cart.Id);
        await _cartRepository.Received(1).CreateAsync(Arg.Any<Cart>(), Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Given invalid cart data When creating cart Then throws validation exception")]
    public async Task Handle_InvalidRequest_ThrowsValidationException()
    {
        // Given
        var command = new CreateCartCommand(); // empty command fails validation (no products)

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<FluentValidation.ValidationException>();
    }

    [Fact(DisplayName = "Given valid command When handling Then mapper is called with command")]
    public async Task Handle_ValidRequest_MapsCommandToCart()
    {
        // Given
        var command = CreateCartHandlerTestData.GenerateValidCommand();
        var cart = new Cart
        {
            Id = Guid.NewGuid(),
            UserId = command.UserId,
            Date = command.Date,
            Products = []
        };

        _mapper.Map<Cart>(command).Returns(cart);
        _cartRepository.CreateAsync(Arg.Any<Cart>(), Arg.Any<CancellationToken>()).Returns(cart);

        // When
        await _handler.Handle(command, CancellationToken.None);

        // Then
        _mapper.Received(1).Map<Cart>(Arg.Is<CreateCartCommand>(c =>
            c.UserId == command.UserId &&
            c.Date == command.Date));
    }
}
