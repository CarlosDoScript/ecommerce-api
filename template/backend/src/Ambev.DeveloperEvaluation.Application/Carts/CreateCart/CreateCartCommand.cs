using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Carts.CreateCart;

public record CreateCartCommand : IRequest<CreateCartResult>
{
    public Guid UserId { get; init; }
    public DateTime Date { get; init; }
    public List<CreateCartItemCommand> Products { get; init; } = [];
}

public record CreateCartItemCommand
{
    public Guid ProductId { get; init; }
    public int Quantity { get; init; }
}
