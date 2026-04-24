using Ambev.DeveloperEvaluation.Application.Carts.UpdateCart;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Unit.Application.Carts.TestData;
using AutoMapper;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Carts;

/// <summary>
/// Contains unit tests for the <see cref="UpdateCartHandler"/> class.
/// </summary>
public class UpdateCartHandlerTests
{
    private readonly ICartRepository _cartRepository;
    private readonly IMapper _mapper;
    private readonly UpdateCartHandler _handler;

    public UpdateCartHandlerTests()
    {
        _cartRepository = Substitute.For<ICartRepository>();
        _mapper = Substitute.For<IMapper>();
        _handler = new UpdateCartHandler(_cartRepository, _mapper);
    }

    [Fact(DisplayName = "Given valid update data When cart exists Then returns updated cart")]
    public async Task Handle_ValidCommand_ExistingCart_ReturnsUpdatedResult()
    {
        // Given
        var command = UpdateCartHandlerTestData.GenerateValidCommand();
        var existingCart = new Cart
        {
            Id = command.Id,
            UserId = Guid.NewGuid(),
            Date = DateTime.UtcNow.AddDays(-1),
            Products = new List<CartItem>()
        };
        var updatedResult = new UpdateCartResult { Id = command.Id, UserId = command.UserId };

        _cartRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>()).Returns(existingCart);
        _cartRepository.UpdateAsync(Arg.Any<Cart>(), Arg.Any<CancellationToken>()).Returns(existingCart);
        _mapper.Map<List<CartItem>>(Arg.Any<List<UpdateCartItemCommand>>()).Returns(new List<CartItem>());
        _mapper.Map<UpdateCartResult>(Arg.Any<Cart>()).Returns(updatedResult);

        // When
        var result = await _handler.Handle(command, CancellationToken.None);

        // Then
        result.Should().NotBeNull();
        result.Id.Should().Be(command.Id);
        await _cartRepository.Received(1).UpdateAsync(Arg.Any<Cart>(), Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Given non-existing cart ID When updating Then throws KeyNotFoundException")]
    public async Task Handle_NonExistingCart_ThrowsKeyNotFoundException()
    {
        // Given
        var command = UpdateCartHandlerTestData.GenerateValidCommand();
        _cartRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>()).Returns((Cart?)null);

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage($"*{command.Id}*");
    }

    [Fact(DisplayName = "Given invalid command When updating Then throws ValidationException")]
    public async Task Handle_InvalidCommand_ThrowsValidationException()
    {
        // Given
        var command = new UpdateCartCommand(); // no products, empty userId

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<FluentValidation.ValidationException>();
    }

    [Fact(DisplayName = "Given valid command When updating Then cart fields are persisted")]
    public async Task Handle_ValidCommand_UpdatesCartFields()
    {
        // Given
        var newUserId = Guid.NewGuid();
        var newDate = DateTime.UtcNow;
        var command = UpdateCartHandlerTestData.GenerateValidCommand() with
        {
            UserId = newUserId,
            Date = newDate
        };

        var existingCart = new Cart { Id = command.Id, UserId = Guid.NewGuid(), Date = DateTime.UtcNow.AddDays(-5), Products = [] };

        _cartRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>()).Returns(existingCart);
        _cartRepository.UpdateAsync(Arg.Any<Cart>(), Arg.Any<CancellationToken>())
            .Returns(ci => ci.Arg<Cart>());
        _mapper.Map<List<CartItem>>(Arg.Any<List<UpdateCartItemCommand>>()).Returns(new List<CartItem>());
        _mapper.Map<UpdateCartResult>(Arg.Any<Cart>()).Returns(new UpdateCartResult());

        // When
        await _handler.Handle(command, CancellationToken.None);

        // Then
        await _cartRepository.Received(1).UpdateAsync(
            Arg.Is<Cart>(c =>
                c.UserId == newUserId &&
                c.Date == newDate &&
                c.UpdatedAt != null),
            Arg.Any<CancellationToken>());
    }
}
