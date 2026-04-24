using AutoMapper;
using MediatR;
using Ambev.DeveloperEvaluation.Domain.Repositories;

namespace Ambev.DeveloperEvaluation.Application.Carts.ListCarts;

public class ListCartsHandler : IRequestHandler<ListCartsCommand, ListCartsResult>
{
    private readonly ICartRepository _cartRepository;
    private readonly IMapper _mapper;

    public ListCartsHandler(ICartRepository cartRepository, IMapper mapper)
    {
        _cartRepository = cartRepository;
        _mapper = mapper;
    }

    public async Task<ListCartsResult> Handle(ListCartsCommand request, CancellationToken cancellationToken)
    {
        var (items, totalCount) = await _cartRepository.GetPagedAsync(
            request.Page, request.Size, request.Order,
            request.UserId, request.MinDate, request.MaxDate,
            cancellationToken);

        var totalPages = (int)Math.Ceiling(totalCount / (double)request.Size);

        return new ListCartsResult
        {
            Data = _mapper.Map<IEnumerable<CartListItem>>(items),
            TotalItems = totalCount,
            CurrentPage = request.Page,
            TotalPages = totalPages
        };
    }
}
