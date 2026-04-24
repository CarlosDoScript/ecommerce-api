using AutoMapper;
using Ambev.DeveloperEvaluation.Application.Products.UpdateProduct;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Products.UpdateProduct;

public class UpdateProductWebProfile : Profile
{
    public UpdateProductWebProfile()
    {
        CreateMap<UpdateProductResult, UpdateProductResponse>()
            .ForMember(dest => dest.Rating, opt => opt.MapFrom(src => src.Rating));
        CreateMap<UpdateProductRatingResult, UpdateProductRatingResponse>();
    }
}
