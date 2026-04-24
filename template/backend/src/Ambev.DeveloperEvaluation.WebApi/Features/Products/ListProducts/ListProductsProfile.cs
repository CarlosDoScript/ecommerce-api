using AutoMapper;
using Ambev.DeveloperEvaluation.Application.Products.ListProducts;
using Ambev.DeveloperEvaluation.Application.Products.ListProductsByCategory;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Products.ListProducts;

public class ListProductsWebProfile : Profile
{
    public ListProductsWebProfile()
    {
        CreateMap<ProductListItem, ListProductsResponseItem>()
            .ForMember(dest => dest.Rating, opt => opt.MapFrom(src => src.Rating));
        CreateMap<ProductListRating, ListProductsRatingResponse>();
        CreateMap<ProductByCategoryItem, ListProductsResponseItem>()
            .ForMember(dest => dest.Rating, opt => opt.MapFrom(src => src.Rating));
        CreateMap<ProductByCategoryRating, ListProductsRatingResponse>();
    }
}
