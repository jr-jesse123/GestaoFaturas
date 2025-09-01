using GestaoFaturas.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace GestaoFaturas.Api.Data.Repositories;

public class InvoiceStatusRepository : Repository<InvoiceStatus>, IInvoiceStatusRepository
{
    public InvoiceStatusRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<InvoiceStatus?> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(s => s.Name == name, cancellationToken);
    }

    public async Task<IEnumerable<InvoiceStatus>> GetActiveStatusesAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(s => s.IsActive)
            .OrderBy(s => s.SortOrder)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<InvoiceStatus>> GetFinalStatusesAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(s => s.IsFinal)
            .OrderBy(s => s.SortOrder)
            .ToListAsync(cancellationToken);
    }

    public async Task<InvoiceStatus?> GetDefaultStatusAsync(CancellationToken cancellationToken = default)
    {
        // Get the first active status (usually "Pending")
        return await _dbSet
            .Where(s => s.IsActive)
            .OrderBy(s => s.SortOrder)
            .FirstOrDefaultAsync(cancellationToken);
    }
}