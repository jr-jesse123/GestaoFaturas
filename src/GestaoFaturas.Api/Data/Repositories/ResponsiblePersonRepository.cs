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

    // New methods for Client-based operations
    public async Task<IEnumerable<ResponsiblePerson>> GetByClientAsync(int clientId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(p => p.ClientId == clientId)
            .Include(p => p.Client)
            .OrderBy(p => p.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<ResponsiblePerson?> GetPrimaryByClientAsync(int clientId, CancellationToken cancellationToken = default)
    {
        // Primary is now handled via CostCenterResponsibles
        var primaryAssignment = await _context.CostCenterResponsibles
            .Where(ccr => ccr.IsPrimary && ccr.ResponsiblePerson.ClientId == clientId)
            .Include(ccr => ccr.ResponsiblePerson)
            .ThenInclude(rp => rp.Client)
            .Select(ccr => ccr.ResponsiblePerson)
            .FirstOrDefaultAsync(cancellationToken);

        return primaryAssignment;
    }
}