using Ambev.DeveloperEvaluation.Application.Sales.Events;
using Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Unit.Application.Sales.TestData;
using AutoMapper;
using FluentAssertions;
using FluentValidation;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales;

public class UpdateSaleHandlerTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly ISaleEventPublisher _eventPublisher;
    private readonly UpdateSaleHandler _handler;

    public UpdateSaleHandlerTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _mapper = Substitute.For<IMapper>();
        _eventPublisher = Substitute.For<ISaleEventPublisher>();
        _handler = new UpdateSaleHandler(_saleRepository, _mapper, _eventPublisher);
    }

    [Fact(DisplayName = "Given valid command When updating sale Then returns updated result")]
    public async Task Handle_ValidCommand_ReturnsUpdatedResult()
    {
        // Given
        var saleId = Guid.NewGuid();
        var command = UpdateSaleHandlerTestData.GenerateCommandWithId(saleId);

        var existingSale = new Sale
        {
            Id = saleId,
            SaleNumber = "20260423-ABCD1234",
            CustomerName = "Old Customer",
            Items = []
        };
        var updatedSale = new Sale
        {
            Id = saleId,
            SaleNumber = existingSale.SaleNumber,
            CustomerName = command.CustomerName,
            Items = []
        };
        var result = new UpdateSaleResult { Id = saleId, CustomerName = command.CustomerName };

        _saleRepository.GetByIdAsync(saleId, Arg.Any<CancellationToken>()).Returns(existingSale);
        _mapper.Map<List<SaleItem>>(command.Items).Returns([]);
        _saleRepository.UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>()).Returns(updatedSale);
        _mapper.Map<UpdateSaleResult>(updatedSale).Returns(result);

        // When
        var updateResult = await _handler.Handle(command, CancellationToken.None);

        // Then
        updateResult.Should().NotBeNull();
        updateResult.Id.Should().Be(saleId);
        await _saleRepository.Received(1).UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Given valid command When updating sale Then publishes SaleModified event")]
    public async Task Handle_ValidCommand_PublishesSaleModifiedEvent()
    {
        // Given
        var saleId = Guid.NewGuid();
        var command = UpdateSaleHandlerTestData.GenerateCommandWithId(saleId);

        var existingSale = new Sale { Id = saleId, SaleNumber = "20260423-ABCD1234", Items = [] };
        var updatedSale = new Sale { Id = saleId, SaleNumber = existingSale.SaleNumber, Items = [] };
        var result = new UpdateSaleResult { Id = saleId };

        _saleRepository.GetByIdAsync(saleId, Arg.Any<CancellationToken>()).Returns(existingSale);
        _mapper.Map<List<SaleItem>>(command.Items).Returns([]);
        _saleRepository.UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>()).Returns(updatedSale);
        _mapper.Map<UpdateSaleResult>(updatedSale).Returns(result);

        // When
        await _handler.Handle(command, CancellationToken.None);

        // Then
        _eventPublisher.Received(1).PublishSaleModified(saleId, updatedSale.SaleNumber);
    }

    [Fact(DisplayName = "Given newly cancelled item When updating sale Then publishes ItemCancelled event")]
    public async Task Handle_NewlyCancelledItem_PublishesItemCancelledEvent()
    {
        // Given
        var saleId = Guid.NewGuid();
        var productId = Guid.NewGuid();
        var command = UpdateSaleHandlerTestData.GenerateCommandWithId(saleId) with
        {
            Items =
            [
                new UpdateSaleItemCommand
                {
                    ProductId = productId,
                    ProductName = "Cancelled Product",
                    UnitPrice = 50m,
                    Quantity = 1,
                    IsCancelled = true
                }
            ]
        };

        var existingSale = new Sale { Id = saleId, SaleNumber = "20260423-ABCD1234", Items = [] };
        var cancelledItem = new SaleItem { ProductId = productId, ProductName = "Cancelled Product", IsCancelled = true };
        var updatedSale = new Sale
        {
            Id = saleId,
            SaleNumber = existingSale.SaleNumber,
            Items = [cancelledItem]
        };
        var result = new UpdateSaleResult { Id = saleId };

        _saleRepository.GetByIdAsync(saleId, Arg.Any<CancellationToken>()).Returns(existingSale);
        _mapper.Map<List<SaleItem>>(command.Items).Returns([cancelledItem]);
        _saleRepository.UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>()).Returns(updatedSale);
        _mapper.Map<UpdateSaleResult>(updatedSale).Returns(result);

        // When
        await _handler.Handle(command, CancellationToken.None);

        // Then
        _eventPublisher.Received(1).PublishItemCancelled(saleId, productId, "Cancelled Product");
    }

    [Fact(DisplayName = "Given non-existing sale ID When updating Then throws KeyNotFoundException")]
    public async Task Handle_NonExistingSale_ThrowsKeyNotFoundException()
    {
        // Given
        var saleId = Guid.NewGuid();
        var command = UpdateSaleHandlerTestData.GenerateCommandWithId(saleId);
        _saleRepository.GetByIdAsync(saleId, Arg.Any<CancellationToken>()).Returns((Sale?)null);

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<KeyNotFoundException>()
            .WithMessage($"*{saleId}*");
    }

    [Fact(DisplayName = "Given invalid command When updating sale Then throws ValidationException")]
    public async Task Handle_InvalidCommand_ThrowsValidationException()
    {
        // Given
        var command = new UpdateSaleCommand(); // empty — fails validation

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<ValidationException>();
    }
}
