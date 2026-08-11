using StoreApp.Domain.Events.DomainEvent;

namespace StoreApp.Application.Basket.Events
{
    /// <summary>
    /// رویداد اضافه شدن یک کالا به سبد خرید
    /// </summary>
    public   class BasketItemAddedEvent : IDomainEvent
    {
        public long BasketId { get; }
        public long UserId { get; }
        public long ProductId { get; }
        public int Quantity { get; }

        public BasketItemAddedEvent(long basketId,long userId,long productId,int quantity)
        {
            BasketId = basketId;
            UserId = userId;
            ProductId = productId;
            Quantity = quantity;
        }
    }
}