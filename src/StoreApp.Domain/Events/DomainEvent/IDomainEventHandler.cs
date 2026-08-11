using System;
using System.Collections.Generic;
using System.Text;

namespace StoreApp.Domain.Events.DomainEvent
{
    //چه کسی یک Event مشخص را پردازش می‌کند
    public interface IDomainEventHandler<TEvent>  where TEvent : IDomainEvent
    {
        Task HandleAsync(TEvent domainEvent,CancellationToken cancellationToken = default);
    }
}
