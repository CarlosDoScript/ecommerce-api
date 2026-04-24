using FluentValidation;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Products.UpdateProduct;

public class UpdateProductRequestValidator : AbstractValidator<UpdateProductRequest>
{
    public UpdateProductRequestValidator()
    {
        RuleFor(p => p.Title).NotEmpty().MaximumLength(200);
        RuleFor(p => p.Price).GreaterThan(0);
        RuleFor(p => p.Category).NotEmpty().MaximumLength(100);
        RuleFor(p => p.Description).NotEmpty();
        RuleFor(p => p.Rating.Rate).InclusiveBetween(0, 5);
        RuleFor(p => p.Rating.Count).GreaterThanOrEqualTo(0);
    }
}
