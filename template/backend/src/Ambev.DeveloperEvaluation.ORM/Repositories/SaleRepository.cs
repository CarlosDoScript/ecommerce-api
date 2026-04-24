using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Ambev.DeveloperEvaluation.ORM.Repositories;

public class SaleRepository : ISaleRepository
{
    private readonly DefaultContext _context;

    public SaleRepository(DefaultContext context)
    {
        _context = context;
    }

    public async Task<Sale> CreateAsync(Sale sale, CancellationToken cancellationToken = default)
    {
        await _context.Sales.AddAsync(sale, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return sale;
    }

    public async Task<Sale?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Sales
            .Include(s => s.Items)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public async Task<Sale> UpdateAsync(Sale sale, CancellationToken cancellationToken = default)
    {
        _context.Sales.Update(sale);
        await _context.SaveChangesAsync(cancellationToken);
        return sale;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var sale = await GetByIdAsync(id, cancellationToken);
        if (sale == null)
            return false;

        _context.Sales.Remove(sale);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<(IEnumerable<Sale> Items, int TotalCount)> GetPagedAsync(
        int page, int size, string? order,
        Guid? customerId, Guid? branchId, bool? isCancelled,
        DateTime? minDate, DateTime? maxDate,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Sales.AsNoTracking().Include(s => s.Items).AsQueryable();

        if (customerId.HasValue)
            query = query.Where(s => s.CustomerId == customerId.Value);

        if (branchId.HasValue)
            query = query.Where(s => s.BranchId == branchId.Value);

        if (isCancelled.HasValue)
            query = query.Where(s => s.IsCancelled == isCancelled.Value);

        if (minDate.HasValue)
            query = query.Where(s => s.Date >= minDate.Value);

        if (maxDate.HasValue)
            query = query.Where(s => s.Date <= maxDate.Value);

        var totalCount = await query.CountAsync(cancellationToken);
        query = ApplyOrder(query, order);
        var items = await query.Skip((page - 1) * size).Take(size).ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    private static IQueryable<Sale> ApplyOrder(IQueryable<Sale> query, string? order)
    {
        if (string.IsNullOrWhiteSpace(order))
            return query.OrderByDescending(s => s.Date);

        var parts = order.Trim('"').Trim('\'').Split(',');
        IOrderedQueryable<Sale>? ordered = null;

        foreach (var part in parts)
        {
            var tokens = part.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var field = tokens[0].Trim().ToLower();
            var dir = tokens.Length > 1 ? tokens[1].Trim().ToLower() : "asc";

            if (ordered == null)
            {
                ordered = (field, dir) switch
                {
                    ("date", "desc")         => query.OrderByDescending(s => s.Date),
                    ("date", _)              => query.OrderBy(s => s.Date),
                    ("salenumber", "desc")   => query.OrderByDescending(s => s.SaleNumber),
                    ("salenumber", _)        => query.OrderBy(s => s.SaleNumber),
                    ("customername", "desc") => query.OrderByDescending(s => s.CustomerName),
                    ("customername", _)      => query.OrderBy(s => s.CustomerName),
                    ("branchname", "desc")   => query.OrderByDescending(s => s.BranchName),
                    ("branchname", _)        => query.OrderBy(s => s.BranchName),
                    ("totalamount", "desc")  => query.OrderByDescending(s => s.TotalAmount),
                    ("totalamount", _)       => query.OrderBy(s => s.TotalAmount),
                    (_, "desc")              => query.OrderByDescending(s => s.Date),
                    _                        => query.OrderBy(s => s.Date)
                };
            }
            else
            {
                ordered = (field, dir) switch
                {
                    ("date", "desc")         => ordered.ThenByDescending(s => s.Date),
                    ("date", _)              => ordered.ThenBy(s => s.Date),
                    ("salenumber", "desc")   => ordered.ThenByDescending(s => s.SaleNumber),
                    ("salenumber", _)        => ordered.ThenBy(s => s.SaleNumber),
                    ("customername", "desc") => ordered.ThenByDescending(s => s.CustomerName),
                    ("customername", _)      => ordered.ThenBy(s => s.CustomerName),
                    ("totalamount", "desc")  => ordered.ThenByDescending(s => s.TotalAmount),
                    ("totalamount", _)       => ordered.ThenBy(s => s.TotalAmount),
                    (_, "desc")              => ordered.ThenByDescending(s => s.Date),
                    _                        => ordered.ThenBy(s => s.Date)
                };
            }
        }

        return ordered ?? query.OrderByDescending(s => s.Date);
    }
}
