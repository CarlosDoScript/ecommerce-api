using AutoMapper;
using Ambev.DeveloperEvaluation.Application.Users.ListUsers;
using Ambev.DeveloperEvaluation.Domain.ValueObjects;
using Ambev.DeveloperEvaluation.WebApi.Features.Users.Common;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Users.ListUsers;

public class ListUsersWebProfile : Profile
{
    public ListUsersWebProfile()
    {
        CreateMap<UserName, UserNameModel>().ReverseMap();
        CreateMap<UserAddress, UserAddressModel>()
            .ForMember(d => d.Geolocation, o => o.MapFrom(s => new UserGeolocationModel { Lat = s.Lat, Long = s.Long }));

        CreateMap<UserListItem, ListUsersResponseItem>();
    }
}
