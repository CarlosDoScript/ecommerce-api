using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Domain.ValueObjects;

namespace Ambev.DeveloperEvaluation.Application.Users.ListUsers;

public class ListUsersResult
{
    public IEnumerable<UserListItem> Data { get; set; } = [];
    public int TotalItems { get; set; }
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
}

public class UserListItem
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
