using AutoMapper;
using MediatR;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;

namespace Ambev.DeveloperEvaluation.Application.Sales.ListSales;

public class ListSalesHandler : IRequestHandler<ListSalesCommand, ListSalesResult>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;

    public ListSalesHandler(ISaleRepository saleRepository, IMapper mapper)
    {
        _saleRepository = saleRepository;
        _mapper = mapper;
    }

    public async Task<ListSalesResult> Handle(ListSalesCommand command, CancellationToken cancellationToken)
    {
        var (items, totalCount) = await _saleRepository.GetPagedAsync(
            command.Page, command.Size, command.Order,
            command.CustomerId, command.BranchId, command.IsCancelled,
            command.MinDate, command.MaxDate,
            cancellationToken);

        return new ListSalesResult
        {
            Data = _mapper.Map<IEnumerable<SaleListItem>>(items),
            TotalItems = totalCount,
            CurrentPage = command.Page,
            TotalPages = (int)Math.Ceiling(totalCount / (double)command.Size)
        };
    }
}
