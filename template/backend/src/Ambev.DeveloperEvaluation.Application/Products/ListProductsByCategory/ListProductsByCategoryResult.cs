namespace Ambev.DeveloperEvaluation.Application.Products.ListProductsByCategory;

public class ListProductsByCategoryResult
{
    public IEnumerable<ProductByCategoryItem> Data { get; set; } = [];
    public int TotalItems { get; set; }
    public int CurrentPage { get; set; }
    public int TotalPages { get; set; }
}

public class ProductByCategoryItem
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Image { get; set; } = string.Empty;
    public ProductByCategoryRating Rating { get; set; } = new();
}

public class ProductByCategoryRating
{
    public decimal Rate { get; set; }
    public int Count { get; set; }
}
