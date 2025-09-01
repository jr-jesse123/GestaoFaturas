using GestaoFaturas.Api.Models;

namespace GestaoFaturas.Api.Data.Repositories;

public interface IResponsiblePersonRepository : IRepository<ResponsiblePerson>
{
    Task<IEnumerable<ResponsiblePerson>> GetByCostCenterAsync(int costCenterId, CancellationToken cancellationToken = default);
    Task<ResponsiblePerson?> GetPrimaryByCostCenterAsync(int costCenterId, CancellationToken cancellationToken = default);
    Task<ResponsiblePerson?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<IEnumerable<ResponsiblePerson>> GetActivePersonsAsync(CancellationToken cancellationToken = default);
}