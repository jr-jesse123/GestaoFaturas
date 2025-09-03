using GestaoFaturas.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace GestaoFaturas.Api.Data.Repositories;

public class ClientRepository : Repository<Client>, IClientRepository
{
    public ClientRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Client?> GetByTaxIdAsync(string taxId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(c => c.TaxId == taxId, cancellationToken);
    }

    public async Task<IEnumerable<Client>> GetActiveClientsAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(c => c.IsActive)
            .OrderBy(c => c.CompanyName)
            .ToListAsync(cancellationToken);
    }

    public async Task<Client?> GetClientWithCostCentersAsync(int clientId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(c => c.CostCenters)
                .ThenInclude(cc => cc.CostCenterResponsibles)
                    .ThenInclude(ccr => ccr.ResponsiblePerson)
            .FirstOrDefaultAsync(c => c.Id == clientId, cancellationToken);
    }

    public async Task<Client?> GetClientWithInvoicesAsync(int clientId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(c => c.Invoices)
                .ThenInclude(i => i.InvoiceStatus)
            .Include(c => c.Invoices)
                .ThenInclude(i => i.CostCenter)
            .FirstOrDefaultAsync(c => c.Id == clientId, cancellationToken);
    }

    public async Task<bool> HasInvoicesAsync(int clientId, CancellationToken cancellationToken = default)
    {
        return await _context.Invoices
            .AnyAsync(i => i.ClientId == clientId, cancellationToken);
    }
}