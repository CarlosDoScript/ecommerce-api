namespace Ambev.DeveloperEvaluation.Application.Carts.ListCarts;

public class ListCartsResult
{
    public IEnumerable<CartListItem> Data { get; set; } = [];
    public int TotalItems { get; set; }
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
}

public class CartListItem
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public DateTime Date { get; set; }
    public List<CartListItemProduct> Products { get; set; } = [];
}

public class CartListItemProduct
{
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
}
