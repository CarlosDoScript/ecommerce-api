using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Ambev.DeveloperEvaluation.ORM.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly DefaultContext _context;

    public ProductRepository(DefaultContext context)
    {
        _context = context;
    }

    public async Task<Product> CreateAsync(Product product, CancellationToken cancellationToken = default)
    {
        await _context.Products.AddAsync(product, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return product;
    }

    public async Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Products.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<Product> UpdateAsync(Product product, CancellationToken cancellationToken = default)
    {
        _context.Products.Update(product);
        await _context.SaveChangesAsync(cancellationToken);
        return product;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var product = await GetByIdAsync(id, cancellationToken);
        if (product == null)
            return false;

        _context.Products.Remove(product);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<(IEnumerable<Product> Items, int TotalCount)> GetPagedAsync(
        int page, int size, string? order,
        string? title, string? category, decimal? minPrice, decimal? maxPrice,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Products.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(title))
        {
            var t = title.Trim('*');
            if (title.StartsWith('*') && title.EndsWith('*'))
                query = query.Where(p => p.Title.Contains(t));
            else if (title.StartsWith('*'))
                query = query.Where(p => p.Title.EndsWith(t));
            else if (title.EndsWith('*'))
                query = query.Where(p => p.Title.StartsWith(t));
            else
                query = query.Where(p => p.Title == title);
        }

        if (!string.IsNullOrWhiteSpace(category))
        {
            var c = category.Trim('*');
            if (category.StartsWith('*') && category.EndsWith('*'))
                query = query.Where(p => p.Category.Contains(c));
            else if (category.StartsWith('*'))
                query = query.Where(p => p.Category.EndsWith(c));
            else if (category.EndsWith('*'))
                query = query.Where(p => p.Category.StartsWith(c));
            else
                query = query.Where(p => p.Category == category);
        }

        if (minPrice.HasValue)
            query = query.Where(p => p.Price >= minPrice.Value);

        if (maxPrice.HasValue)
            query = query.Where(p => p.Price <= maxPrice.Value);

        var totalCount = await query.CountAsync(cancellationToken);
        query = ApplyOrder(query, order);
        var items = await query.Skip((page - 1) * size).Take(size).ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<IEnumerable<string>> GetCategoriesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Products.AsNoTracking()
            .Select(p => p.Category)
            .Distinct()
            .OrderBy(c => c)
            .ToListAsync(cancellationToken);
    }

    public async Task<(IEnumerable<Product> Items, int TotalCount)> GetByCategoryPagedAsync(
        string category, int page, int size, string? order,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Products.AsNoTracking()
            .Where(p => p.Category == category);

        var totalCount = await query.CountAsync(cancellationToken);
        query = ApplyOrder(query, order);
        var items = await query.Skip((page - 1) * size).Take(size).ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    private static IQueryable<Product> ApplyOrder(IQueryable<Product> query, string? order)
    {
        if (string.IsNullOrWhiteSpace(order))
            return query.OrderBy(p => p.Id);

        var parts = order.Trim('"').Trim('\'').Split(',');
        IOrderedQueryable<Product>? ordered = null;

        foreach (var part in parts)
        {
            var tokens = part.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var field = tokens[0].Trim().ToLower();
            var dir = tokens.Length > 1 ? tokens[1].Trim().ToLower() : "asc";

            if (ordered == null)
            {
                ordered = (field, dir) switch
                {
                    ("title", "desc") => query.OrderByDescending(p => p.Title),
                    ("title", _) => query.OrderBy(p => p.Title),
                    ("price", "desc") => query.OrderByDescending(p => p.Price),
                    ("price", _) => query.OrderBy(p => p.Price),
                    ("category", "desc") => query.OrderByDescending(p => p.Category),
                    ("category", _) => query.OrderBy(p => p.Category),
                    (_, "desc") => query.OrderByDescending(p => p.Id),
                    _ => query.OrderBy(p => p.Id)
                };
            }
            else
            {
                ordered = (field, dir) switch
                {
                    ("title", "desc") => ordered.ThenByDescending(p => p.Title),
                    ("title", _) => ordered.ThenBy(p => p.Title),
                    ("price", "desc") => ordered.ThenByDescending(p => p.Price),
                    ("price", _) => ordered.ThenBy(p => p.Price),
                    ("category", "desc") => ordered.ThenByDescending(p => p.Category),
                    ("category", _) => ordered.ThenBy(p => p.Category),
                    (_, "desc") => ordered.ThenByDescending(p => p.Id),
                    _ => ordered.ThenBy(p => p.Id)
                };
            }
        }

        return ordered ?? query.OrderBy(p => p.Id);
    }
}
