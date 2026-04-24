using AutoMapper;
using Ambev.DeveloperEvaluation.Domain.ValueObjects;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Users.Common;

public class UserValueObjectsProfile : Profile
{
    public UserValueObjectsProfile()
    {
        CreateMap<UserNameModel, UserName>().ReverseMap();

        CreateMap<UserAddressModel, UserAddress>()
            .ForMember(d => d.Lat, o => o.MapFrom(s => s.Geolocation.Lat))
            .ForMember(d => d.Long, o => o.MapFrom(s => s.Geolocation.Long));

        CreateMap<UserAddress, UserAddressModel>()
            .ForMember(d => d.Geolocation, o => o.MapFrom(s => new UserGeolocationModel { Lat = s.Lat, Long = s.Long }));
    }
}
