using MediatR;

namespace Ambev.DeveloperEvaluation.Application.Carts.GetCart;

public record GetCartCommand(Guid Id) : IRequest<GetCartResult>;
