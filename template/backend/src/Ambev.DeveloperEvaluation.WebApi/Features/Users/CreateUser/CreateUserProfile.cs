using AutoMapper;
using Ambev.DeveloperEvaluation.Application.Users.CreateUser;
using Ambev.DeveloperEvaluation.Domain.ValueObjects;
using Ambev.DeveloperEvaluation.WebApi.Features.Users.Common;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Users.CreateUser;

public class CreateUserProfile : Profile
{
    public CreateUserProfile()
    {
        CreateMap<UserNameModel, UserName>().ReverseMap();
        CreateMap<UserAddressModel, UserAddress>()
            .ForMember(d => d.Lat, o => o.MapFrom(s => s.Geolocation.Lat))
            .ForMember(d => d.Long, o => o.MapFrom(s => s.Geolocation.Long));
        CreateMap<UserAddress, UserAddressModel>()
            .ForMember(d => d.Geolocation, o => o.MapFrom(s => new UserGeolocationModel { Lat = s.Lat, Long = s.Long }));

        CreateMap<CreateUserRequest, CreateUserCommand>();
        CreateMap<CreateUserResult, CreateUserResponse>();
    }
}
