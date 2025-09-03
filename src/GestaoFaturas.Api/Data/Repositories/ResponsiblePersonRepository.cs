using GestaoFaturas.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace GestaoFaturas.Api.Data.Repositories;

public class ResponsiblePersonRepository : Repository<ResponsiblePerson>, IResponsiblePersonRepository
{
    public ResponsiblePersonRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<ResponsiblePerson>> GetByCostCenterAsync(int costCenterId, CancellationToken cancellationToken = default)
    {
        return await _context.CostCenterResponsibles
            .Where(ccr => ccr.CostCenterId == costCenterId)
            .Include(ccr => ccr.ResponsiblePerson)
            .ThenInclude(rp => rp.Client)
            .OrderBy(ccr => ccr.IsPrimary ? 0 : 1)
            .ThenBy(ccr => ccr.ResponsiblePerson.Name)
            .Select(ccr => ccr.ResponsiblePerson)
            .ToListAsync(cancellationToken);
    }

    public async Task<ResponsiblePerson?> GetPrimaryByCostCenterAsync(int costCenterId, CancellationToken cancellationToken = default)
    {
        return await _context.CostCenterResponsibles
            .Where(ccr => ccr.CostCenterId == costCenterId && ccr.IsPrimary)
            .Include(ccr => ccr.ResponsiblePerson)
            .ThenInclude(rp => rp.Client)
            .Select(ccr => ccr.ResponsiblePerson)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<ResponsiblePerson?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(p => p.Email == email)
            .Include(p => p.Client)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IEnumerable<ResponsiblePerson>> GetActivePersonsAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(p => p.IsActive)
            .Include(p => p.Client)
            .OrderBy(p => p.Name)
            .ToListAsync(cancellationToken);
    }

    // Client-based operations
    public async Task<ResponsiblePerson?> GetByIdWithClientAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(p => p.Id == id)
            .Include(p => p.Client)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IEnumerable<ResponsiblePerson>> GetByClientIdAsync(int clientId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(p => p.ClientId == clientId)
            .Include(p => p.Client)
            .OrderBy(p => p.IsPrimaryContact ? 0 : 1)
            .ThenBy(p => p.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<ResponsiblePerson>> GetActiveByClientIdAsync(int clientId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(p => p.ClientId == clientId && p.IsActive)
            .Include(p => p.Client)
            .OrderBy(p => p.IsPrimaryContact ? 0 : 1)
            .ThenBy(p => p.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<ResponsiblePerson?> GetPrimaryContactAsync(int clientId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(p => p.ClientId == clientId && p.IsPrimaryContact)
            .Include(p => p.Client)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<bool> HasPrimaryContactAsync(int clientId, int? excludeId = null, CancellationToken cancellationToken = default)
    {
        var query = _dbSet.Where(p => p.ClientId == clientId && p.IsPrimaryContact);
        
        if (excludeId.HasValue)
        {
            query = query.Where(p => p.Id != excludeId.Value);
        }

        return await query.AnyAsync(cancellationToken);
    }

    public async Task<bool> EmailExistsAsync(string email, int clientId, int? excludeId = null, CancellationToken cancellationToken = default)
    {
        var query = _dbSet.Where(p => p.Email == email && p.ClientId == clientId);
        
        if (excludeId.HasValue)
        {
            query = query.Where(p => p.Id != excludeId.Value);
        }

        return await query.AnyAsync(cancellationToken);
    }
}