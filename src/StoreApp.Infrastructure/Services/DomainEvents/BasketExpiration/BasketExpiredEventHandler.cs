using Microsoft.Extensions.Logging;
using StoreApp.Domain.Events.DomainEvent;
 

namespace StoreApp.Domain.Events.Basket
{


    /// <summary>
    /// /. رویدادها پیاده سازی
    /// </summary>
    public sealed class BasketExpiredEventHandler: IDomainEventHandler<BasketExpiredEvent>
    {
        private readonly ILogger<BasketExpiredEventHandler> _logger;

        public BasketExpiredEventHandler(ILogger<BasketExpiredEventHandler> logger)
        {
            _logger = logger;
        }

        public Task HandleAsync(BasketExpiredEvent domainEvent,CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("سبد خرید با شناسه {BasketId} برای کاربر {UserId} منقضی شد.",domainEvent.BasketId,domainEvent.UserId);

            return Task.CompletedTask;
        }
    }
}
