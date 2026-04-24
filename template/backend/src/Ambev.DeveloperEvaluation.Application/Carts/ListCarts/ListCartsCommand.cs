using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Carts.ListCarts;

public record ListCartsCommand : IRequest<ListCartsResult>
{
    public int Page { get; init; } = 1;
    public int Size { get; init; } = 10;
    public string? Order { get; init; }
    public Guid? UserId { get; init; }
    public DateTime? MinDate { get; init; }
    public DateTime? MaxDate { get; init; }
}
