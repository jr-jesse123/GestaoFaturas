using GestaoFaturas.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace GestaoFaturas.Api.Data.Repositories;

public class InvoiceHistoryRepository : Repository<InvoiceHistory>, IInvoiceHistoryRepository
{
    public InvoiceHistoryRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<InvoiceHistory>> GetByInvoiceAsync(int invoiceId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(h => h.InvoiceId == invoiceId)
            .Include(h => h.FromStatus)
            .Include(h => h.ToStatus)
            .OrderByDescending(h => h.ChangedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<InvoiceHistory>> GetByUserAsync(string userId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(h => h.ChangedByUserId == userId)
            .Include(h => h.Invoice)
                .ThenInclude(i => i.Client)
            .Include(h => h.FromStatus)
            .Include(h => h.ToStatus)
            .OrderByDescending(h => h.ChangedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<InvoiceHistory>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(h => h.ChangedAt >= startDate && h.ChangedAt <= endDate)
            .Include(h => h.Invoice)
                .ThenInclude(i => i.Client)
            .Include(h => h.FromStatus)
            .Include(h => h.ToStatus)
            .OrderByDescending(h => h.ChangedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<InvoiceHistory?> GetLatestByInvoiceAsync(int invoiceId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(h => h.InvoiceId == invoiceId)
            .Include(h => h.FromStatus)
            .Include(h => h.ToStatus)
            .OrderByDescending(h => h.ChangedAt)
            .FirstOrDefaultAsync(cancellationToken);
    }
}