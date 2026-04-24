using AutoMapper;
using Ambev.DeveloperEvaluation.Application.Carts.ListCarts;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Carts.ListCarts;

public class ListCartsWebProfile : Profile
{
    public ListCartsWebProfile()
    {
        CreateMap<CartListItem, ListCartsResponseItem>()
            .ForMember(dest => dest.Products, opt => opt.MapFrom(src => src.Products));
        CreateMap<CartListItemProduct, ListCartsItemProductResponse>();
    }
}
