using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Products.CreateProduct;

public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(p => p.Title).NotEmpty().MaximumLength(200);
        RuleFor(p => p.Price).GreaterThan(0);
        RuleFor(p => p.Category).NotEmpty().MaximumLength(100);
        RuleFor(p => p.Description).NotEmpty();
        RuleFor(p => p.Rating.Rate).InclusiveBetween(0, 5);
        RuleFor(p => p.Rating.Count).GreaterThanOrEqualTo(0);
    }
}
