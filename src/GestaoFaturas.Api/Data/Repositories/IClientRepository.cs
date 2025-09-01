using GestaoFaturas.Api.Models;

namespace GestaoFaturas.Api.Data.Repositories;

public interface IClientRepository : IRepository<Client>
{
    Task<Client?> GetByTaxIdAsync(string taxId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Client>> GetActiveClientsAsync(CancellationToken cancellationToken = default);
    Task<Client?> GetClientWithCostCentersAsync(int clientId, CancellationToken cancellationToken = default);
    Task<Client?> GetClientWithInvoicesAsync(int clientId, CancellationToken cancellationToken = default);
    Task<bool> HasInvoicesAsync(int clientId, CancellationToken cancellationToken = default);
}