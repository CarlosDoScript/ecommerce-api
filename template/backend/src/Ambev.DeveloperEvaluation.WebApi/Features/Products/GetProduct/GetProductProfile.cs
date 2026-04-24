using AutoMapper;
using Ambev.DeveloperEvaluation.Application.Products.GetProduct;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Products.GetProduct;

public class GetProductWebProfile : Profile
{
    public GetProductWebProfile()
    {
        CreateMap<GetProductResult, GetProductResponse>()
            .ForMember(dest => dest.Rating, opt => opt.MapFrom(src => src.Rating));
        CreateMap<GetProductRatingResult, GetProductRatingResponse>();
    }
}
