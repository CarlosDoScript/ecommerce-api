using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Domain.ValueObjects;
using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Users.UpdateUser;

public class UpdateUserCommand : IRequest<UpdateUserResult>
{
    public Guid Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public UserStatus Status { get; set; }
    public UserRole Role { get; set; }
    public UserName Name { get; set; } = new();
    public UserAddress? Address { get; set; }
}
