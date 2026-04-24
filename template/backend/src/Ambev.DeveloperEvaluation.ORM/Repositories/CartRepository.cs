using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Ambev.DeveloperEvaluation.ORM.Repositories;

public class CartRepository : ICartRepository
{
    private readonly DefaultContext _context;

    public CartRepository(DefaultContext context)
    {
        _context = context;
    }

    public async Task<Cart> CreateAsync(Cart cart, CancellationToken cancellationToken = default)
    {
        await _context.Carts.AddAsync(cart, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return cart;
    }

    public async Task<Cart?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Carts
            .Include(c => c.Products)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<Cart> UpdateAsync(Cart cart, CancellationToken cancellationToken = default)
    {
        _context.Carts.Update(cart);
        await _context.SaveChangesAsync(cancellationToken);
        return cart;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var cart = await GetByIdAsync(id, cancellationToken);
        if (cart == null)
            return false;

        _context.Carts.Remove(cart);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<(IEnumerable<Cart> Items, int TotalCount)> GetPagedAsync(
        int page, int size, string? order,
        Guid? userId, DateTime? minDate, DateTime? maxDate,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Carts.AsNoTracking()
            .Include(c => c.Products)
            .AsQueryable();

        if (userId.HasValue)
            query = query.Where(c => c.UserId == userId.Value);

        if (minDate.HasValue)
            query = query.Where(c => c.Date >= minDate.Value);

        if (maxDate.HasValue)
            query = query.Where(c => c.Date <= maxDate.Value);

        var totalCount = await query.CountAsync(cancellationToken);
        query = ApplyOrder(query, order);
        var items = await query.Skip((page - 1) * size).Take(size).ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    private static IQueryable<Cart> ApplyOrder(IQueryable<Cart> query, string? order)
    {
        if (string.IsNullOrWhiteSpace(order))
            return query.OrderBy(c => c.Id);

        var parts = order.Trim('"').Trim('\'').Split(',');
        IOrderedQueryable<Cart>? ordered = null;

        foreach (var part in parts)
        {
            var tokens = part.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var field = tokens[0].Trim().ToLower();
            var dir = tokens.Length > 1 ? tokens[1].Trim().ToLower() : "asc";

            if (ordered == null)
            {
                ordered = (field, dir) switch
                {
                    ("userid", "desc") => query.OrderByDescending(c => c.UserId),
                    ("userid", _) => query.OrderBy(c => c.UserId),
                    ("date", "desc") => query.OrderByDescending(c => c.Date),
                    ("date", _) => query.OrderBy(c => c.Date),
                    (_, "desc") => query.OrderByDescending(c => c.Id),
                    _ => query.OrderBy(c => c.Id)
                };
            }
            else
            {
                ordered = (field, dir) switch
                {
                    ("userid", "desc") => ordered.ThenByDescending(c => c.UserId),
                    ("userid", _) => ordered.ThenBy(c => c.UserId),
                    ("date", "desc") => ordered.ThenByDescending(c => c.Date),
                    ("date", _) => ordered.ThenBy(c => c.Date),
                    (_, "desc") => ordered.ThenByDescending(c => c.Id),
                    _ => ordered.ThenBy(c => c.Id)
                };
            }
        }

        return ordered ?? query.OrderBy(c => c.Id);
    }
}
