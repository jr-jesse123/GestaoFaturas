using GestaoFaturas.Api.Models;

namespace GestaoFaturas.Api.Data.Repositories;

public interface IResponsiblePersonRepository : IRepository<ResponsiblePerson>
{
    // Client-based operations
    Task<ResponsiblePerson?> GetByIdWithClientAsync(int id, CancellationToken cancellationToken = default);
    Task<IEnumerable<ResponsiblePerson>> GetByClientIdAsync(int clientId, CancellationToken cancellationToken = default);
    Task<IEnumerable<ResponsiblePerson>> GetActiveByClientIdAsync(int clientId, CancellationToken cancellationToken = default);
    Task<ResponsiblePerson?> GetPrimaryContactAsync(int clientId, CancellationToken cancellationToken = default);
    Task<bool> HasPrimaryContactAsync(int clientId, int? excludeId = null, CancellationToken cancellationToken = default);
    Task<bool> EmailExistsAsync(string email, int clientId, int? excludeId = null, CancellationToken cancellationToken = default);
    
    // CostCenter-based operations
    Task<IEnumerable<ResponsiblePerson>> GetByCostCenterAsync(int costCenterId, CancellationToken cancellationToken = default);
    Task<ResponsiblePerson?> GetPrimaryByCostCenterAsync(int costCenterId, CancellationToken cancellationToken = default);
    
    // General operations
    Task<ResponsiblePerson?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<IEnumerable<ResponsiblePerson>> GetActivePersonsAsync(CancellationToken cancellationToken = default);
}