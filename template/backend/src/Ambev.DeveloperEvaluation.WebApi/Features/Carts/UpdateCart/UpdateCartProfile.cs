using AutoMapper;
using Ambev.DeveloperEvaluation.Application.Carts.UpdateCart;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Carts.UpdateCart;

public class UpdateCartWebProfile : Profile
{
    public UpdateCartWebProfile()
    {
        CreateMap<UpdateCartResult, UpdateCartResponse>()
            .ForMember(dest => dest.Products, opt => opt.MapFrom(src => src.Products));
        CreateMap<UpdateCartItemResult, UpdateCartItemResponse>();
    }
}
