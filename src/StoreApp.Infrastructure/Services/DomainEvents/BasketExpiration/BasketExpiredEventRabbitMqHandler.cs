using Microsoft.Extensions.Logging;
using StoreApp.Application.Abstractions.MessageBus;
using StoreApp.Domain.Events.Basket;
using StoreApp.Domain.Events.DomainEvent;

namespace StoreApp.Infrastructure.Services.DomainEvents.BasketExpiration
{
    public sealed class BasketExpiredEventRabbitMqHandler: IDomainEventHandler<BasketExpiredEvent>
    {
        private readonly IBasketEventPublisher _eventPublisher;
        private readonly ILogger<BasketExpiredEventRabbitMqHandler> _logger;

        public BasketExpiredEventRabbitMqHandler(IBasketEventPublisher eventPublisher,ILogger<BasketExpiredEventRabbitMqHandler> logger)
        {
            _eventPublisher = eventPublisher;
            _logger = logger;
        }

        public async Task HandleAsync(BasketExpiredEvent domainEvent,CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("سبد خرید با شناسه {BasketId} برای کاربر {UserId} منقضی شد.",domainEvent.BasketId,domainEvent.UserId);

            await _eventPublisher.PublishAsync(domainEvent,cancellationToken);
        }
    }
}