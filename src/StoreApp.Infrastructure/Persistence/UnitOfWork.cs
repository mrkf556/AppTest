using Microsoft.EntityFrameworkCore.Storage;
using StoreApp.Application.Abstractions.DependencyInjection;
using StoreApp.Domain.Events.DomainEvent;
 
namespace StoreApp.Infrastructure.Persistence
{
    public class UnitOfWork : IUnitOfWork, IScopedDependency
    {
        private readonly BasketDbContext _context;
        private IDbContextTransaction? _transaction;

        public UnitOfWork(BasketDbContext context)
        {
            _context = context;
        }
        public IReadOnlyCollection<IDomainEvent> GetDomainEvents()
        {
            return _context.GetDomainEvents();
        }
        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return _context.SaveChangesAsync(cancellationToken);
        }

        public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            _transaction =await _context.Database.BeginTransactionAsync(cancellationToken);
        }

        public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (_transaction is null)
                return;

            await _transaction.CommitAsync(cancellationToken);

            await _transaction.DisposeAsync();

            _transaction = null;
        }

        public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
        {
            if (_transaction is null)
                return;

            await _transaction.RollbackAsync(cancellationToken);

            await _transaction.DisposeAsync();

            _transaction = null;
        }
    }
}