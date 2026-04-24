using Ambev.DeveloperEvaluation.Application.Sales.DeleteSale;
using Ambev.DeveloperEvaluation.Application.Sales.Events;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales;

public class DeleteSaleHandlerTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly ISaleEventPublisher _eventPublisher;
    private readonly DeleteSaleHandler _handler;

    public DeleteSaleHandlerTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _eventPublisher = Substitute.For<ISaleEventPublisher>();
        _handler = new DeleteSaleHandler(_saleRepository, _eventPublisher);
    }

    [Fact(DisplayName = "Given existing sale When cancelling Then returns success")]
    public async Task Handle_ExistingSale_ReturnsSuccess()
    {
        // Given
        var saleId = Guid.NewGuid();
        var sale = new Sale { Id = saleId, SaleNumber = "20260423-ABCD1234" };
        var cancelledSale = new Sale { Id = saleId, SaleNumber = sale.SaleNumber, IsCancelled = true };

        _saleRepository.GetByIdAsync(saleId, Arg.Any<CancellationToken>()).Returns(sale);
        _saleRepository.UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>()).Returns(cancelledSale);

        // When
        var result = await _handler.Handle(new DeleteSaleCommand(saleId), CancellationToken.None);

        // Then
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();
    }

    [Fact(DisplayName = "Given existing sale When cancelling Then sale IsCancelled is set to true")]
    public async Task Handle_ExistingSale_SetsSaleCancelled()
    {
        // Given
        var saleId = Guid.NewGuid();
        var sale = new Sale { Id = saleId, SaleNumber = "20260423-ABCD1234", IsCancelled = false };

        _saleRepository.GetByIdAsync(saleId, Arg.Any<CancellationToken>()).Returns(sale);
        _saleRepository.UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>())
            .Returns(ci => ci.ArgAt<Sale>(0));

        // When
        await _handler.Handle(new DeleteSaleCommand(saleId), CancellationToken.None);

        // Then
        await _saleRepository.Received(1).UpdateAsync(
            Arg.Is<Sale>(s => s.IsCancelled == true),
            Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Given existing sale When cancelling Then publishes SaleCancelled event")]
    public async Task Handle_ExistingSale_PublishesSaleCancelledEvent()
    {
        // Given
        var saleId = Guid.NewGuid();
        var saleNumber = "20260423-ABCD1234";
        var sale = new Sale { Id = saleId, SaleNumber = saleNumber };
        var cancelledSale = new Sale { Id = saleId, SaleNumber = saleNumber, IsCancelled = true };

        _saleRepository.GetByIdAsync(saleId, Arg.Any<CancellationToken>()).Returns(sale);
        _saleRepository.UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>()).Returns(cancelledSale);

        // When
        await _handler.Handle(new DeleteSaleCommand(saleId), CancellationToken.None);

        // Then
        _eventPublisher.Received(1).PublishSaleCancelled(saleId, saleNumber);
    }

    [Fact(DisplayName = "Given non-existing sale ID When cancelling Then throws KeyNotFoundException")]
    public async Task Handle_NonExistingSale_ThrowsKeyNotFoundException()
    {
        // Given
        var saleId = Guid.NewGuid();
        _saleRepository.GetByIdAsync(saleId, Arg.Any<CancellationToken>()).Returns((Sale?)null);

        // When
        var act = () => _handler.Handle(new DeleteSaleCommand(saleId), CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage($"*{saleId}*");
    }
}
