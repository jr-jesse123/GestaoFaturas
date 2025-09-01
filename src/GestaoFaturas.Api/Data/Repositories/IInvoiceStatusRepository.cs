using GestaoFaturas.Api.Models;

namespace GestaoFaturas.Api.Data.Repositories;

public interface IInvoiceStatusRepository : IRepository<InvoiceStatus>
{
    Task<InvoiceStatus?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<IEnumerable<InvoiceStatus>> GetActiveStatusesAsync(CancellationToken cancellationToken = default);
    Task<IEnumerable<InvoiceStatus>> GetFinalStatusesAsync(CancellationToken cancellationToken = default);
    Task<InvoiceStatus?> GetDefaultStatusAsync(CancellationToken cancellationToken = default);
}