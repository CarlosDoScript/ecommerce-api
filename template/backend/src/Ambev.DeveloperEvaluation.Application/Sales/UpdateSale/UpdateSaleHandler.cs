using AutoMapper;
using FluentValidation;
using MediatR;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Application.Sales.Events;

namespace Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;

public class UpdateSaleHandler : IRequestHandler<UpdateSaleCommand, UpdateSaleResult>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly ISaleEventPublisher _eventPublisher;

    public UpdateSaleHandler(ISaleRepository saleRepository, IMapper mapper, ISaleEventPublisher eventPublisher)
    {
        _saleRepository = saleRepository;
        _mapper = mapper;
        _eventPublisher = eventPublisher;
    }

    public async Task<UpdateSaleResult> Handle(UpdateSaleCommand command, CancellationToken cancellationToken)
    {
        var validator = new UpdateSaleCommandValidator();
        var validationResult = await validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var sale = await _saleRepository.GetByIdAsync(command.Id, cancellationToken);
        if (sale == null)
            throw new KeyNotFoundException($"Sale with ID {command.Id} not found.");

        var previouslyCancelledIds = sale.Items
            .Where(i => i.IsCancelled)
            .Select(i => i.ProductId)
            .ToHashSet();

        sale.Date = command.Date;
        sale.CustomerId = command.CustomerId;
        sale.CustomerName = command.CustomerName;
        sale.BranchId = command.BranchId;
        sale.BranchName = command.BranchName;
        sale.Items = _mapper.Map<List<SaleItem>>(command.Items);
        sale.UpdatedAt = DateTime.UtcNow;
        sale.CalculateTotals();

        var updated = await _saleRepository.UpdateAsync(sale, cancellationToken);

        _eventPublisher.PublishSaleModified(updated.Id, updated.SaleNumber);

        foreach (var item in updated.Items.Where(i => i.IsCancelled && !previouslyCancelledIds.Contains(i.ProductId)))
            _eventPublisher.PublishItemCancelled(updated.Id, item.ProductId, item.ProductName);

        return _mapper.Map<UpdateSaleResult>(updated);
    }
}
