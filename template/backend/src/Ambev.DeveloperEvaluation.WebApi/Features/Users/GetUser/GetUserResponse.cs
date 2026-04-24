using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.WebApi.Features.Users.Common;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Users.GetUser;

public class GetUserResponse
{
    public Guid Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public UserNameModel Name { get; set; } = new();
    public UserAddressModel? Address { get; set; }
    public UserRole Role { get; set; }
    public UserStatus Status { get; set; }
}
