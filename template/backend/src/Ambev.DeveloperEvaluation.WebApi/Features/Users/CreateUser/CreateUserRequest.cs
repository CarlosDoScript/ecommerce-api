using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.WebApi.Features.Users.Common;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Users.CreateUser;

public class CreateUserRequest
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public UserStatus Status { get; set; }
    public UserRole Role { get; set; }
    public UserNameModel Name { get; set; } = new();
    public UserAddressModel? Address { get; set; }
}