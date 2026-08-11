using StoreApp.Domain.Events.DomainEvent;
using System;
using System.Collections.Generic;
using System.Text;

namespace StoreApp.Domain.Events.Basket
{
    // این رویداد پس از تغییر وضعیت Basket به Expired ایجاد می‌شود
    // تا بخش‌های دیگر سیستم بتوانند به این اتفاق واکنش نشان دهند
    public class BasketExpiredEvent : IDomainEvent
    {
        public long BasketId { get; }
        public long UserId { get; }

        public BasketExpiredEvent(long basketId, long userId)
        {
            BasketId = basketId;
            UserId = userId;
        }
    }
}
