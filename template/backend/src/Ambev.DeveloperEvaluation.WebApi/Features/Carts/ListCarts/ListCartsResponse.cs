namespace Ambev.DeveloperEvaluation.WebApi.Features.Carts.ListCarts;

public class ListCartsResponseItem
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public DateTime Date { get; set; }
    public List<ListCartsItemProductResponse> Products { get; set; } = [];
}

public class ListCartsItemProductResponse
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
}
