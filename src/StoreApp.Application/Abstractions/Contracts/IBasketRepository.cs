using StoreApp.Domain.Enitities;
using System;
using System.Collections.Generic;
using System.Text;
using BasketEntity = StoreApp.Domain.Enitities.Basket;


namespace StoreApp.Application.Abstractions.Contracts
{
    // این Interface قرارداد دسترسی اختصاصی به داده‌های Basket را تعریف می‌کند
    // علاوه بر عملیات عمومی Repository، Queryهای مخصوص Basket
    // مانند دریافت سبد فعال یک کاربر را مشخص می‌کند
    public interface IBasketRepository : IRepository<BasketEntity>
    {

        //این متد برای لو د کردن دیتا هست قبل از هر عملی ایتم های بسکت
        Task<BasketEntity?> GetActiveBasketByUserIdAsync(long userId,CancellationToken cancellationToken = default);
        ///ما برای اینکه لیست بسکت های اکتیو رو به دست بیاریم و همچنین زمان ان نسبت به 30 دقیقه
        Task<List<BasketEntity>> GetExpiredBasketsAsync(DateTime expirationTime,CancellationToken cancellationToken = default);
    }
}
