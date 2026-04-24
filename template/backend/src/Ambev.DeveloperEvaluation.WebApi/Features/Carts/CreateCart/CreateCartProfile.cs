using AutoMapper;
using Ambev.DeveloperEvaluation.Application.Carts.CreateCart;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Carts.CreateCart;

public class CreateCartWebProfile : Profile
{
    public CreateCartWebProfile()
    {
        CreateMap<CreateCartRequest, CreateCartCommand>()
            .ForMember(dest => dest.Products, opt => opt.MapFrom(src => src.Products));
        CreateMap<CreateCartItemRequest, CreateCartItemCommand>();
        CreateMap<CreateCartResult, CreateCartResponse>()
            .ForMember(dest => dest.Products, opt => opt.MapFrom(src => src.Products));
        CreateMap<CreateCartItemResult, CreateCartItemResponse>();
    }
}
