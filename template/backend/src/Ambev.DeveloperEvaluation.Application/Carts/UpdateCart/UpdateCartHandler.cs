using AutoMapper;
using MediatR;
using FluentValidation;
using Ambev.DeveloperEvaluation.Domain.Repositories;

namespace Ambev.DeveloperEvaluation.Application.Carts.UpdateCart;

public class UpdateCartHandler : IRequestHandler<UpdateCartCommand, UpdateCartResult>
{
    private readonly ICartRepository _cartRepository;
    private readonly IMapper _mapper;

    public UpdateCartHandler(ICartRepository cartRepository, IMapper mapper)
    {
        _cartRepository = cartRepository;
        _mapper = mapper;
    }

    public async Task<UpdateCartResult> Handle(UpdateCartCommand command, CancellationToken cancellationToken)
    {
        var validator = new UpdateCartCommandValidator();
        var validationResult = await validator.ValidateAsync(command, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var cart = await _cartRepository.GetByIdAsync(command.Id, cancellationToken);
        if (cart == null)
            throw new KeyNotFoundException($"Cart with ID {command.Id} not found");

        cart.UserId = command.UserId;
        cart.Date = command.Date;
        cart.Products = _mapper.Map<List<Domain.Entities.CartItem>>(command.Products);
        cart.UpdatedAt = DateTime.UtcNow;

        var updated = await _cartRepository.UpdateAsync(cart, cancellationToken);
        return _mapper.Map<UpdateCartResult>(updated);
    }
}
