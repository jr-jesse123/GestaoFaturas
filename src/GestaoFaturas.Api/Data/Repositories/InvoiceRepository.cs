using GestaoFaturas.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace GestaoFaturas.Api.Data.Repositories;

public class InvoiceRepository : Repository<Invoice>, IInvoiceRepository
{
    public InvoiceRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Invoice?> GetByInvoiceNumberAsync(int clientId, string invoiceNumber, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(i => i.Client)
            .Include(i => i.CostCenter)
            .Include(i => i.InvoiceStatus)
            .FirstOrDefaultAsync(i => i.ClientId == clientId && i.InvoiceNumber == invoiceNumber, cancellationToken);
    }

    public async Task<IEnumerable<Invoice>> GetByClientAsync(int clientId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(i => i.ClientId == clientId)
            .Include(i => i.CostCenter)
            .Include(i => i.InvoiceStatus)
            .OrderByDescending(i => i.IssueDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Invoice>> GetByCostCenterAsync(int costCenterId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(i => i.CostCenterId == costCenterId)
            .Include(i => i.Client)
            .Include(i => i.InvoiceStatus)
            .OrderByDescending(i => i.IssueDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Invoice>> GetByStatusAsync(int statusId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(i => i.InvoiceStatusId == statusId)
            .Include(i => i.Client)
            .Include(i => i.CostCenter)
            .OrderByDescending(i => i.IssueDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Invoice>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(i => i.IssueDate >= startDate && i.IssueDate <= endDate)
            .Include(i => i.Client)
            .Include(i => i.CostCenter)
            .Include(i => i.InvoiceStatus)
            .OrderByDescending(i => i.IssueDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Invoice>> GetOverdueInvoicesAsync(CancellationToken cancellationToken = default)
    {
        var today = DateTime.UtcNow.Date;
        
        return await _dbSet
            .Where(i => i.DueDate < today && i.PaidDate == null)
            .Include(i => i.Client)
            .Include(i => i.CostCenter)
            .Include(i => i.InvoiceStatus)
            .OrderBy(i => i.DueDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<Invoice?> GetWithHistoryAsync(int invoiceId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(i => i.Client)
            .Include(i => i.CostCenter)
            .Include(i => i.InvoiceStatus)
            .Include(i => i.InvoiceHistories)
                .ThenInclude(h => h.FromStatus)
            .Include(i => i.InvoiceHistories)
                .ThenInclude(h => h.ToStatus)
            .FirstOrDefaultAsync(i => i.Id == invoiceId, cancellationToken);
    }

    public async Task<decimal> GetTotalAmountByClientAsync(int clientId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(i => i.ClientId == clientId)
            .SumAsync(i => i.TotalAmount, cancellationToken);
    }

    public async Task<decimal> GetTotalAmountByCostCenterAsync(int costCenterId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(i => i.CostCenterId == costCenterId)
            .SumAsync(i => i.TotalAmount, cancellationToken);
    }
}