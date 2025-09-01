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
        return await _dbSet
            .Where(p => p.CostCenterId == costCenterId)
            .Include(p => p.CostCenter)
            .OrderBy(p => p.IsPrimary ? 0 : 1)
            .ThenBy(p => p.FullName)
            .ToListAsync(cancellationToken);
    }

    public async Task<ResponsiblePerson?> GetPrimaryByCostCenterAsync(int costCenterId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(p => p.CostCenterId == costCenterId && p.IsPrimary)
            .Include(p => p.CostCenter)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<ResponsiblePerson?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(p => p.Email == email)
            .Include(p => p.CostCenter)
                .ThenInclude(cc => cc.Client)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IEnumerable<ResponsiblePerson>> GetActivePersonsAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(p => p.IsActive)
            .Include(p => p.CostCenter)
                .ThenInclude(cc => cc.Client)
            .OrderBy(p => p.FullName)
            .ToListAsync(cancellationToken);
    }
}