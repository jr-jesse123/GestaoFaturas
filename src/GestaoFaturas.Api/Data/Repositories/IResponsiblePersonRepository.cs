using GestaoFaturas.Api.Models;

namespace GestaoFaturas.Api.Data.Repositories;

public interface IResponsiblePersonRepository : IRepository<ResponsiblePerson>
{
    // CostCenter-based operations (existing)
    Task<IEnumerable<ResponsiblePerson>> GetByCostCenterAsync(int costCenterId, CancellationToken cancellationToken = default);
    Task<ResponsiblePerson?> GetPrimaryByCostCenterAsync(int costCenterId, CancellationToken cancellationToken = default);
    
    // Client-based operations (new)
    Task<IEnumerable<ResponsiblePerson>> GetByClientAsync(int clientId, CancellationToken cancellationToken = default);
    Task<ResponsiblePerson?> GetPrimaryByClientAsync(int clientId, CancellationToken cancellationToken = default);
    
    // General operations
    Task<ResponsiblePerson?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<IEnumerable<ResponsiblePerson>> GetActivePersonsAsync(CancellationToken cancellationToken = default);
}