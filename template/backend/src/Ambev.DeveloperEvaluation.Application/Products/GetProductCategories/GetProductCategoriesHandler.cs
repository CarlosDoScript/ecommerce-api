using MediatR;
using Ambev.DeveloperEvaluation.Domain.Repositories;

namespace Ambev.DeveloperEvaluation.Application.Products.GetProductCategories;

public class GetProductCategoriesHandler : IRequestHandler<GetProductCategoriesQuery, GetProductCategoriesResult>
{
    private readonly IProductRepository _productRepository;

    public GetProductCategoriesHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<GetProductCategoriesResult> Handle(GetProductCategoriesQuery request, CancellationToken cancellationToken)
    {
        var categories = await _productRepository.GetCategoriesAsync(cancellationToken);
        return new GetProductCategoriesResult { Categories = categories };
    }
}
