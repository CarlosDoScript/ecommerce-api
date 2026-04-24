using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Carts.UpdateCart;

public record UpdateCartCommand : IRequest<UpdateCartResult>
{
    public Guid Id { get; init; }
    public Guid UserId { get; init; }
    public DateTime Date { get; init; }
    public List<UpdateCartItemCommand> Products { get; init; } = [];
}

public record UpdateCartItemCommand
{
    public Guid ProductId { get; init; }
    public int Quantity { get; init; }
}
