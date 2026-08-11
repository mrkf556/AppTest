using StoreApp.Application.Abstractions.DependencyInjection;
using StoreApp.Application.Abstractions.Results;
using StoreApp.Domain.Events.DomainEvent;
using StoreApp.Infrastructure.Persistence;

namespace StoreApp.Application.Abstractions.Pipeline
{
    public   class TransactionBehavior<TRequest, TResponse>: IPipelineBehavior<TRequest, TResponse>, IScopedDependency where TRequest : class where TResponse : ServiceResult
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDomainEventDispatcher _domainEventDispatcher;

        public TransactionBehavior(IUnitOfWork unitOfWork,IDomainEventDispatcher domainEventDispatcher)
        {
            _unitOfWork = unitOfWork;
            _domainEventDispatcher = domainEventDispatcher;
        }

        public async Task<TResponse> Handle(TRequest request,CancellationToken cancellationToken,Func<Task<TResponse>> next)
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);

            try
            {
                ///با شروع یک کامند تراکنش ایجاد می شود 
                var response = await next();

                var domainEvents =_unitOfWork.GetDomainEvents();

                if (domainEvents.Count > 0)
                {
                    await _domainEventDispatcher.DispatchAsync(domainEvents,cancellationToken);
                }

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                await _unitOfWork.CommitTransactionAsync(cancellationToken);
                //این بعد از اتمام کامند انجام می شود
                return response;
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);

                throw;
            }
        }
    }
}