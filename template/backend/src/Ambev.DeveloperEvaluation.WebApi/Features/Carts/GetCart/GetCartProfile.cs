using AutoMapper;
using Ambev.DeveloperEvaluation.Application.Carts.GetCart;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Carts.GetCart;

public class GetCartWebProfile : Profile
{
    public GetCartWebProfile()
    {
        CreateMap<GetCartResult, GetCartResponse>()
            .ForMember(dest => dest.Products, opt => opt.MapFrom(src => src.Products));
        CreateMap<GetCartItemResult, GetCartItemResponse>();
    }
}
