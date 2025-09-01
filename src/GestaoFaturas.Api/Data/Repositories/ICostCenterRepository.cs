using GestaoFaturas.Api.Models;

namespace GestaoFaturas.Api.Data.Repositories;

public interface ICostCenterRepository : IRepository<CostCenter>
{
    Task<CostCenter?> GetByCodeAsync(int clientId, string code, CancellationToken cancellationToken = default);
    Task<IEnumerable<CostCenter>> GetByClientAsync(int clientId, CancellationToken cancellationToken = default);
    Task<IEnumerable<CostCenter>> GetHierarchyAsync(int rootCostCenterId, CancellationToken cancellationToken = default);
    Task<CostCenter?> GetWithResponsiblePersonsAsync(int costCenterId, CancellationToken cancellationToken = default);
    Task<bool> HasChildrenAsync(int costCenterId, CancellationToken cancellationToken = default);
}