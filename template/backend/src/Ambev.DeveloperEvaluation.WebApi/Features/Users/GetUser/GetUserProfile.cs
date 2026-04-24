using AutoMapper;
using Ambev.DeveloperEvaluation.Application.Users.GetUser;
using Ambev.DeveloperEvaluation.Domain.ValueObjects;
using Ambev.DeveloperEvaluation.WebApi.Features.Users.Common;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Users.GetUser;

public class GetUserProfile : Profile
{
    public GetUserProfile()
    {
        CreateMap<Guid, GetUserCommand>()
            .ConstructUsing(id => new GetUserCommand(id));

        CreateMap<UserName, UserNameModel>().ReverseMap();
        CreateMap<UserAddress, UserAddressModel>()
            .ForMember(d => d.Geolocation, o => o.MapFrom(s => new UserGeolocationModel { Lat = s.Lat, Long = s.Long }));

        CreateMap<GetUserResult, GetUserResponse>();
    }
}
