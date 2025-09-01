using GestaoFaturas.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace GestaoFaturas.Api.Data.Repositories;

public class CostCenterRepository : Repository<CostCenter>, ICostCenterRepository
{
    public CostCenterRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<CostCenter?> GetByCodeAsync(int clientId, string code, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .FirstOrDefaultAsync(cc => cc.ClientId == clientId && cc.Code == code, cancellationToken);
    }

    public async Task<IEnumerable<CostCenter>> GetByClientAsync(int clientId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(cc => cc.ClientId == clientId)
            .Include(cc => cc.ResponsiblePersons)
            .OrderBy(cc => cc.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<CostCenter>> GetHierarchyAsync(int rootCostCenterId, CancellationToken cancellationToken = default)
    {
        var allCostCenters = await _dbSet
            .Include(cc => cc.ChildCostCenters)
            .ToListAsync(cancellationToken);

        var rootCostCenter = allCostCenters.FirstOrDefault(cc => cc.Id == rootCostCenterId);
        if (rootCostCenter == null)
            return Enumerable.Empty<CostCenter>();

        var result = new List<CostCenter>();
        AddToHierarchy(result, rootCostCenter, allCostCenters);
        
        return result;
    }

    private void AddToHierarchy(List<CostCenter> result, CostCenter current, List<CostCenter> allCostCenters)
    {
        result.Add(current);
        var children = allCostCenters.Where(cc => cc.ParentCostCenterId == current.Id);
        foreach (var child in children)
        {
            AddToHierarchy(result, child, allCostCenters);
        }
    }

    public async Task<CostCenter?> GetWithResponsiblePersonsAsync(int costCenterId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Include(cc => cc.ResponsiblePersons)
            .FirstOrDefaultAsync(cc => cc.Id == costCenterId, cancellationToken);
    }

    public async Task<bool> HasChildrenAsync(int costCenterId, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .AnyAsync(cc => cc.ParentCostCenterId == costCenterId, cancellationToken);
    }
}