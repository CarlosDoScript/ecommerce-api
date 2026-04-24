using Ambev.DeveloperEvaluation.Application.Sales.GetSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using AutoMapper;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales;

public class GetSaleHandlerTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly GetSaleHandler _handler;

    public GetSaleHandlerTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _mapper = Substitute.For<IMapper>();
        _handler = new GetSaleHandler(_saleRepository, _mapper);
    }

    [Fact(DisplayName = "Given existing sale ID When getting sale Then returns sale result")]
    public async Task Handle_ExistingSale_ReturnsSaleResult()
    {
        // Given
        var saleId = Guid.NewGuid();
        var sale = new Sale
        {
            Id = saleId,
            SaleNumber = "20260423-ABCD1234",
            CustomerName = "John Doe",
            BranchName = "Branch A",
            TotalAmount = 100m
        };
        var result = new GetSaleResult { Id = saleId, SaleNumber = sale.SaleNumber, CustomerName = sale.CustomerName };

        _saleRepository.GetByIdAsync(saleId, Arg.Any<CancellationToken>()).Returns(sale);
        _mapper.Map<GetSaleResult>(sale).Returns(result);

        // When
        var response = await _handler.Handle(new GetSaleCommand(saleId), CancellationToken.None);

        // Then
        response.Should().NotBeNull();
        response.Id.Should().Be(saleId);
        response.CustomerName.Should().Be("John Doe");
    }

    [Fact(DisplayName = "Given non-existing sale ID When getting sale Then throws KeyNotFoundException")]
    public async Task Handle_NonExistingSale_ThrowsKeyNotFoundException()
    {
        // Given
        var saleId = Guid.NewGuid();
        _saleRepository.GetByIdAsync(saleId, Arg.Any<CancellationToken>()).Returns((Sale?)null);

        // When
        var act = () => _handler.Handle(new GetSaleCommand(saleId), CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage($"*{saleId}*");
    }

    [Fact(DisplayName = "Given existing sale When getting Then repository is called with correct ID")]
    public async Task Handle_ExistingSale_CallsRepositoryWithCorrectId()
    {
        // Given
        var saleId = Guid.NewGuid();
        var sale = new Sale { Id = saleId };
        var result = new GetSaleResult { Id = saleId };

        _saleRepository.GetByIdAsync(saleId, Arg.Any<CancellationToken>()).Returns(sale);
        _mapper.Map<GetSaleResult>(sale).Returns(result);

        // When
        await _handler.Handle(new GetSaleCommand(saleId), CancellationToken.None);

        // Then
        await _saleRepository.Received(1).GetByIdAsync(saleId, Arg.Any<CancellationToken>());
    }
}
