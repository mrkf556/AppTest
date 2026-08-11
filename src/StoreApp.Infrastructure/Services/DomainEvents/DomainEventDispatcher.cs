using Microsoft.Extensions.DependencyInjection;
using StoreApp.Application.Abstractions.DependencyInjection;
using StoreApp.Domain.Events.DomainEvent;
using System;
using System.Collections.Generic;
using System.Text;

namespace StoreApp.Infrastructure.Services.DomainEvents
{
    //Eventهایی که داخل Domain ایجاد شده‌اند
    public   class DomainEventDispatcher : IDomainEventDispatcher, IScopedDependency
    {
        private readonly IServiceProvider _serviceProvider;

        public DomainEventDispatcher(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        //لیست از Eventها می‌گیرد
        public async Task DispatchAsync(IEnumerable<IDomainEvent> domainEvents,CancellationToken cancellationToken = default)
        {
            foreach (var domainEvent in domainEvents)
            {

                var handlerType = typeof(IDomainEventHandler<>).MakeGenericType(domainEvent.GetType());

                var handlers = _serviceProvider.GetServices(handlerType);

                foreach (var handler in handlers)
                {
                    await ((dynamic)handler).HandleAsync((dynamic)domainEvent,cancellationToken);
                }
            }
        }
    }
}
