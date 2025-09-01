using GestaoFaturas.Api.Models;

namespace GestaoFaturas.Api.Data.Repositories;

public interface IInvoiceHistoryRepository : IRepository<InvoiceHistory>
{
    Task<IEnumerable<InvoiceHistory>> GetByInvoiceAsync(int invoiceId, CancellationToken cancellationToken = default);
    Task<IEnumerable<InvoiceHistory>> GetByUserAsync(string userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<InvoiceHistory>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
    Task<InvoiceHistory?> GetLatestByInvoiceAsync(int invoiceId, CancellationToken cancellationToken = default);
}