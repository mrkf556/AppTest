using System;
using System.Collections.Generic;
using System.Text;

namespace StoreApp.Domain.Events.DomainEvent
{
    public interface IDomainEventDispatcher
    {

        ///دلیل ienumberable
        ///چون یک Command ممکن است باعث ایجاد چند Domain Event شود
        Task DispatchAsync(IEnumerable<IDomainEvent> domainEvents, CancellationToken cancellationToken =default);
    }
}
