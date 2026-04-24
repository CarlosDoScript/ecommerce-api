using AutoMapper;
using Ambev.DeveloperEvaluation.Application.Products.CreateProduct;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Products.CreateProduct;

public class CreateProductWebProfile : Profile
{
    public CreateProductWebProfile()
    {
        CreateMap<CreateProductRequest, CreateProductCommand>()
            .ForMember(dest => dest.Rating, opt => opt.MapFrom(src => src.Rating));
        CreateMap<CreateProductRatingRequest, CreateProductRatingCommand>();
        CreateMap<CreateProductResult, CreateProductResponse>()
            .ForMember(dest => dest.Rating, opt => opt.MapFrom(src => src.Rating));
        CreateMap<CreateProductRatingResult, CreateProductRatingResponse>();
    }
}
