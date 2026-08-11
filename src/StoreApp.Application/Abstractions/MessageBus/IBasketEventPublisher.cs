using System;
using System.Collections.Generic;
using System.Text;

namespace StoreApp.Application.Abstractions.MessageBus
{
    public interface IBasketEventPublisher
    {
        Task PublishAsync<TEvent>(TEvent @event,CancellationToken cancellationToken = default);
    }
}
