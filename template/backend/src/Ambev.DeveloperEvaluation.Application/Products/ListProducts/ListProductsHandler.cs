using AutoMapper;
using MediatR;
using Ambev.DeveloperEvaluation.Domain.Repositories;

namespace Ambev.DeveloperEvaluation.Application.Products.ListProducts;

public class ListProductsHandler : IRequestHandler<ListProductsCommand, ListProductsResult>
{
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;

    public ListProductsHandler(IProductRepository productRepository, IMapper mapper)
    {
        _productRepository = productRepository;
        _mapper = mapper;
    }

    public async Task<ListProductsResult> Handle(ListProductsCommand request, CancellationToken cancellationToken)
    {
        var (items, totalCount) = await _productRepository.GetPagedAsync(
            request.Page, request.Size, request.Order,
            request.Title, request.Category, request.MinPrice, request.MaxPrice,
            cancellationToken);

        var totalPages = (int)Math.Ceiling(totalCount / (double)request.Size);

        return new ListProductsResult
        {
            Data = _mapper.Map<IEnumerable<ProductListItem>>(items),
            TotalItems = totalCount,
            CurrentPage = request.Page,
            TotalPages = totalPages
        };
    }
}
