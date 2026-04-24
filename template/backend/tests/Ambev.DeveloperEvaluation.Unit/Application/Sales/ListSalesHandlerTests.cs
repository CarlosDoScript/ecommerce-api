using Ambev.DeveloperEvaluation.Application.Sales.ListSales;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales;

public class ListSalesHandlerTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly ListSalesHandler _handler;

    public ListSalesHandlerTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _mapper = Substitute.For<IMapper>();
        _handler = new ListSalesHandler(_saleRepository, _mapper);
    }

    [Fact(DisplayName = "Given valid query When listing sales Then returns paginated result")]
    public async Task Handle_ValidQuery_ReturnsPaginatedResult()
    {
        // Given
        var command = new ListSalesCommand { Page = 1, Size = 10 };
        var sales = new List<Sale>
        {
            new Sale { Id = Guid.NewGuid(), CustomerName = "Customer A" },
            new Sale { Id = Guid.NewGuid(), CustomerName = "Customer B" }
        };
        var saleListItems = sales.Select(s => new SaleListItem { Id = s.Id, CustomerName = s.CustomerName }).ToList();

        _saleRepository
            .GetPagedAsync(1, 10, null, null, null, null, null, null, Arg.Any<CancellationToken>())
            .Returns((sales.AsEnumerable(), 2));
        _mapper.Map<IEnumerable<SaleListItem>>(Arg.Any<IEnumerable<Sale>>()).Returns(saleListItems);

        // When
        var result = await _handler.Handle(command, CancellationToken.None);

        // Then
        result.Should().NotBeNull();
        result.TotalItems.Should().Be(2);
        result.CurrentPage.Should().Be(1);
        result.Data.Should().HaveCount(2);
    }

    [Fact(DisplayName = "Given total items and page size When listing Then TotalPages is calculated correctly")]
    public async Task Handle_ValidQuery_CalculatesTotalPagesCorrectly()
    {
        // Given
        var command = new ListSalesCommand { Page = 1, Size = 5 };
        var sales = Enumerable.Range(1, 5).Select(_ => new Sale()).ToList();
        var saleListItems = sales.Select(_ => new SaleListItem()).ToList();

        _saleRepository
            .GetPagedAsync(1, 5, null, null, null, null, null, null, Arg.Any<CancellationToken>())
            .Returns((sales.AsEnumerable(), 12));
        _mapper.Map<IEnumerable<SaleListItem>>(Arg.Any<IEnumerable<Sale>>()).Returns(saleListItems);

        // When
        var result = await _handler.Handle(command, CancellationToken.None);

        // Then
        result.TotalPages.Should().Be(3); // ceil(12/5) = 3
    }

    [Fact(DisplayName = "Given empty result When listing Then returns empty data list")]
    public async Task Handle_NoSales_ReturnsEmptyDataList()
    {
        // Given
        var command = new ListSalesCommand { Page = 1, Size = 10 };

        _saleRepository
            .GetPagedAsync(1, 10, null, null, null, null, null, null, Arg.Any<CancellationToken>())
            .Returns((Enumerable.Empty<Sale>(), 0));
        _mapper.Map<IEnumerable<SaleListItem>>(Arg.Any<IEnumerable<Sale>>()).Returns([]);

        // When
        var result = await _handler.Handle(command, CancellationToken.None);

        // Then
        result.TotalItems.Should().Be(0);
        result.TotalPages.Should().Be(0);
        result.Data.Should().BeEmpty();
    }

    [Fact(DisplayName = "Given filter parameters When listing Then passes filters to repository")]
    public async Task Handle_WithFilters_PassesFiltersToRepository()
    {
        // Given
        var customerId = Guid.NewGuid();
        var branchId = Guid.NewGuid();
        var command = new ListSalesCommand
        {
            Page = 2,
            Size = 5,
            CustomerId = customerId,
            BranchId = branchId,
            IsCancelled = false,
            Order = "date desc"
        };

        _saleRepository
            .GetPagedAsync(2, 5, "date desc", customerId, branchId, false, null, null, Arg.Any<CancellationToken>())
            .Returns((Enumerable.Empty<Sale>(), 0));
        _mapper.Map<IEnumerable<SaleListItem>>(Arg.Any<IEnumerable<Sale>>()).Returns([]);

        // When
        await _handler.Handle(command, CancellationToken.None);

        // Then
        await _saleRepository.Received(1).GetPagedAsync(
            2, 5, "date desc", customerId, branchId, false, null, null, Arg.Any<CancellationToken>());
    }
}
