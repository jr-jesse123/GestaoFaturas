namespace GestaoFaturas.Api.Data.Repositories;

public interface IUnitOfWork : IDisposable
{
    // Repository accessors
    IRepository<TEntity> Repository<TEntity>() where TEntity : class;
    
    // Specific repositories (optional, for custom operations)
    IClientRepository Clients { get; }
    ICostCenterRepository CostCenters { get; }
    IInvoiceRepository Invoices { get; }
    IInvoiceStatusRepository InvoiceStatuses { get; }
    IInvoiceHistoryRepository InvoiceHistories { get; }
    IResponsiblePersonRepository ResponsiblePersons { get; }

    // Transaction management
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
    
    // Database operations
    Task<bool> EnsureDatabaseCreatedAsync(CancellationToken cancellationToken = default);
    Task MigrateAsync(CancellationToken cancellationToken = default);
}