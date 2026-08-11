using Microsoft.Extensions.Logging;
using StoreApp.Application.Basket.Events;
using StoreApp.Domain.Events.DomainEvent;

public sealed class BasketItemAddedEventHandler: IDomainEventHandler<BasketItemAddedEvent>
{
    private readonly ILogger<BasketItemAddedEventHandler> _logger;

    public BasketItemAddedEventHandler(ILogger<BasketItemAddedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task HandleAsync(BasketItemAddedEvent domainEvent,CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("محصول {ProductId} با تعداد {Quantity} به سبد {BasketId} برای کاربر {UserId} اضافه شد.",domainEvent.ProductId,domainEvent.Quantity,domainEvent.BasketId,domainEvent.UserId);

        return Task.CompletedTask;
    }
}