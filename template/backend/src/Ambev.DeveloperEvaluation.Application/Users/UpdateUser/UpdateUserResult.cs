using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Domain.ValueObjects;

namespace Ambev.DeveloperEvaluation.Application.Users.UpdateUser;

public class UpdateUserResult
{
    public Guid Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public UserName Name { get; set; } = new();
    public UserAddress? Address { get; set; }
    public UserRole Role { get; set; }
    public UserStatus Status { get; set; }
}
