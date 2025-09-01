using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace GestaoFaturas.Api.Data.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private IDbContextTransaction? _transaction;
    private readonly Dictionary<Type, object> _repositories;

    // Specific repository instances
    private IClientRepository? _clientRepository;
    private ICostCenterRepository? _costCenterRepository;
    private IInvoiceRepository? _invoiceRepository;
    private IInvoiceStatusRepository? _invoiceStatusRepository;
    private IInvoiceHistoryRepository? _invoiceHistoryRepository;
    private IResponsiblePersonRepository? _responsiblePersonRepository;

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _repositories = new Dictionary<Type, object>();
    }

    #region Repository Accessors

    public IRepository<TEntity> Repository<TEntity>() where TEntity : class
    {
        if (_repositories.ContainsKey(typeof(TEntity)))
        {
            return (IRepository<TEntity>)_repositories[typeof(TEntity)];
        }

        var repository = new Repository<TEntity>(_context);
        _repositories.Add(typeof(TEntity), repository);
        return repository;
    }

    public IClientRepository Clients => 
        _clientRepository ??= new ClientRepository(_context);

    public ICostCenterRepository CostCenters => 
        _costCenterRepository ??= new CostCenterRepository(_context);

    public IInvoiceRepository Invoices => 
        _invoiceRepository ??= new InvoiceRepository(_context);

    public IInvoiceStatusRepository InvoiceStatuses => 
        _invoiceStatusRepository ??= new InvoiceStatusRepository(_context);

    public IInvoiceHistoryRepository InvoiceHistories => 
        _invoiceHistoryRepository ??= new InvoiceHistoryRepository(_context);

    public IResponsiblePersonRepository ResponsiblePersons => 
        _responsiblePersonRepository ??= new ResponsiblePersonRepository(_context);

    #endregion

    #region Transaction Management

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException ex)
        {
            // Handle concurrency conflicts
            throw new InvalidOperationException("A concurrency conflict occurred while saving changes.", ex);
        }
        catch (DbUpdateException ex)
        {
            // Handle database update exceptions
            throw new InvalidOperationException("An error occurred while saving changes to the database.", ex);
        }
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction != null)
        {
            throw new InvalidOperationException("A transaction is already in progress.");
        }

        _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction == null)
        {
            throw new InvalidOperationException("No transaction is in progress.");
        }

        try
        {
            await _context.SaveChangesAsync(cancellationToken);
            await _transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await RollbackTransactionAsync(cancellationToken);
            throw;
        }
        finally
        {
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction == null)
        {
            throw new InvalidOperationException("No transaction is in progress.");
        }

        try
        {
            await _transaction.RollbackAsync(cancellationToken);
        }
        finally
        {
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    #endregion

    #region Database Operations

    public async Task<bool> EnsureDatabaseCreatedAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Database.EnsureCreatedAsync(cancellationToken);
    }

    public async Task MigrateAsync(CancellationToken cancellationToken = default)
    {
        await _context.Database.MigrateAsync(cancellationToken);
    }

    #endregion

    #region IDisposable

    private bool _disposed = false;

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _transaction?.Dispose();
                _context.Dispose();
            }
            _disposed = true;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    #endregion
}