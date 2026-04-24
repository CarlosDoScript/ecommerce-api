using AutoMapper;
using MediatR;
using FluentValidation;
using Ambev.DeveloperEvaluation.Domain.Repositories;

namespace Ambev.DeveloperEvaluation.Application.Products.ListProductsByCategory;

public class ListProductsByCategoryHandler : IRequestHandler<ListProductsByCategoryCommand, ListProductsByCategoryResult>
{
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;

    public ListProductsByCategoryHandler(IProductRepository productRepository, IMapper mapper)
    {
        _productRepository = productRepository;
        _mapper = mapper;
    }

    public async Task<ListProductsByCategoryResult> Handle(ListProductsByCategoryCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Category))
            throw new ValidationException("Category is required");

        var (items, totalCount) = await _productRepository.GetByCategoryPagedAsync(
            request.Category, request.Page, request.Size, request.Order, cancellationToken);

        var totalPages = (int)Math.Ceiling(totalCount / (double)request.Size);

        return new ListProductsByCategoryResult
        {
            Data = _mapper.Map<IEnumerable<ProductByCategoryItem>>(items),
            TotalItems = totalCount,
            CurrentPage = request.Page,
            TotalPages = totalPages
        };
    }
}
