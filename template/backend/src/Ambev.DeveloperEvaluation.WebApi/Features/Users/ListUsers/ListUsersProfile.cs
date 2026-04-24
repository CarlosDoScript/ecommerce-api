using AutoMapper;
using Ambev.DeveloperEvaluation.Application.Users.ListUsers;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Users.ListUsers;

public class ListUsersWebProfile : Profile
{
    public ListUsersWebProfile()
    {
        CreateMap<UserListItem, ListUsersResponseItem>();
    }
}
