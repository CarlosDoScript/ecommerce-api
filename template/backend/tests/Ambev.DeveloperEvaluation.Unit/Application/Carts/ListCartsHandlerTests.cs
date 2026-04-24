using Ambev.DeveloperEvaluation.Application.Carts.ListCarts;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Carts;

/// <summary>
/// Contains unit tests for the <see cref="ListCartsHandler"/> class.
/// </summary>
public class ListCartsHandlerTests
{
    private readonly ICartRepository _cartRepository;
    private readonly IMapper _mapper;
    private readonly ListCartsHandler _handler;

    public ListCartsHandlerTests()
    {
        _cartRepository = Substitute.For<ICartRepository>();
        _mapper = Substitute.For<IMapper>();
        _handler = new ListCartsHandler(_cartRepository, _mapper);
    }

    [Fact(DisplayName = "Given valid pagination When listing carts Then returns paginated result")]
    public async Task Handle_ValidPagination_ReturnsPaginatedResult()
    {
        // Given
        var command = new ListCartsCommand { Page = 1, Size = 10 };
        var carts = GenerateCarts(3);
        var mappedItems = carts.Select(c => new CartListItem { Id = c.Id, UserId = c.UserId }).ToList();

        _cartRepository.GetPagedAsync(1, 10, null, null, null, null, Arg.Any<CancellationToken>())
            .Returns((carts, 3));
        _mapper.Map<IEnumerable<CartListItem>>(Arg.Any<IEnumerable<Cart>>()).Returns(mappedItems);

        // When
        var result = await _handler.Handle(command, CancellationToken.None);

        // Then
        result.Should().NotBeNull();
        result.Data.Should().HaveCount(3);
        result.TotalItems.Should().Be(3);
        result.CurrentPage.Should().Be(1);
        result.TotalPages.Should().Be(1);
    }

    [Fact(DisplayName = "Given userId filter When listing carts Then passes userId to repository")]
    public async Task Handle_WithUserIdFilter_PassesFilterToRepository()
    {
        // Given
        var userId = Guid.NewGuid();
        var command = new ListCartsCommand { Page = 1, Size = 10, UserId = userId };

        _cartRepository.GetPagedAsync(1, 10, null, userId, null, null, Arg.Any<CancellationToken>())
            .Returns((Enumerable.Empty<Cart>(), 0));
        _mapper.Map<IEnumerable<CartListItem>>(Arg.Any<IEnumerable<Cart>>())
            .Returns(Enumerable.Empty<CartListItem>());

        // When
        await _handler.Handle(command, CancellationToken.None);

        // Then
        await _cartRepository.Received(1).GetPagedAsync(
            1, 10, null, userId, null, null, Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Given date range filter When listing carts Then passes dates to repository")]
    public async Task Handle_WithDateRangeFilter_PassesDatesToRepository()
    {
        // Given
        var minDate = new DateTime(2024, 1, 1);
        var maxDate = new DateTime(2024, 12, 31);
        var command = new ListCartsCommand { Page = 1, Size = 10, MinDate = minDate, MaxDate = maxDate };

        _cartRepository.GetPagedAsync(1, 10, null, null, minDate, maxDate, Arg.Any<CancellationToken>())
            .Returns((Enumerable.Empty<Cart>(), 0));
        _mapper.Map<IEnumerable<CartListItem>>(Arg.Any<IEnumerable<Cart>>())
            .Returns(Enumerable.Empty<CartListItem>());

        // When
        await _handler.Handle(command, CancellationToken.None);

        // Then
        await _cartRepository.Received(1).GetPagedAsync(
            1, 10, null, null, minDate, maxDate, Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Given total items not fitting one page When listing Then calculates total pages correctly")]
    public async Task Handle_TotalItemsExceedPageSize_CalculatesTotalPagesCorrectly()
    {
        // Given
        var command = new ListCartsCommand { Page = 1, Size = 3 };

        _cartRepository.GetPagedAsync(Arg.Any<int>(), Arg.Any<int>(), Arg.Any<string>(),
                Arg.Any<Guid?>(), Arg.Any<DateTime?>(), Arg.Any<DateTime?>(), Arg.Any<CancellationToken>())
            .Returns((GenerateCarts(3), 7)); // 7 total, size 3 → 3 pages
        _mapper.Map<IEnumerable<CartListItem>>(Arg.Any<IEnumerable<Cart>>())
            .Returns(Enumerable.Empty<CartListItem>());

        // When
        var result = await _handler.Handle(command, CancellationToken.None);

        // Then
        result.TotalPages.Should().Be(3); // ceil(7/3) = 3
        result.TotalItems.Should().Be(7);
    }

    private static List<Cart> GenerateCarts(int count) =>
        Enumerable.Range(0, count).Select(_ => new Cart
        {
            Id = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            Date = DateTime.UtcNow,
            Products = []
        }).ToList();
}
