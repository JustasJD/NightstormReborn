using Microsoft.EntityFrameworkCore.Storage;
using Nightstorm.Core.Interfaces.Repositories;
using Nightstorm.Data.Contexts;

namespace Nightstorm.Data.Repositories;

/// <summary>
/// Unit of Work implementation for managing transactions.
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    private readonly RpgContext _context;
    private IDbContextTransaction? _transaction;
    private ICharacterRepository? _characterRepository;

    public UnitOfWork(RpgContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public ICharacterRepository Characters
    {
        get
        {
            _characterRepository ??= new CharacterRepository(_context);
            return _characterRepository;
        }
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction == null)
        {
            throw new InvalidOperationException("No transaction has been started.");
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
            if (_transaction != null)
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync(cancellationToken);
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
    }
}
