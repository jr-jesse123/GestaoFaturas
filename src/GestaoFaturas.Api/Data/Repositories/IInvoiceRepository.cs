using GestaoFaturas.Api.Models;

namespace GestaoFaturas.Api.Data.Repositories;

public interface IInvoiceRepository : IRepository<Invoice>
{
    Task<Invoice?> GetByInvoiceNumberAsync(int clientId, string invoiceNumber, CancellationToken cancellationToken = default);
    Task<IEnumerable<Invoice>> GetByClientAsync(int clientId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Invoice>> GetByCostCenterAsync(int costCenterId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Invoice>> GetByStatusAsync(int statusId, CancellationToken cancellationToken = default);
    Task<IEnumerable<Invoice>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
    Task<IEnumerable<Invoice>> GetOverdueInvoicesAsync(CancellationToken cancellationToken = default);
    Task<Invoice?> GetWithHistoryAsync(int invoiceId, CancellationToken cancellationToken = default);
    Task<decimal> GetTotalAmountByClientAsync(int clientId, CancellationToken cancellationToken = default);
    Task<decimal> GetTotalAmountByCostCenterAsync(int costCenterId, CancellationToken cancellationToken = default);
}