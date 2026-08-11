using System;
using System.Collections.Generic;
using System.Text;

namespace StoreApp.Application.Basket.DTOs
{
    // فقط برای انتقال داده استفاده می‌شود
    // این DTO اطلاعات مورد نیاز برای افزودن یک کالا به سبد خرید را دریافت می‌کند.
    // این کلاس فقط داده ورودی را حمل می‌کند و اعتبارسنجی اصلی درخواست
    // در لایه Application و Validator انجام خواهد شد.
    public   class AddBasketItemDTO
    {
        public long ProductId { get; set; }

        public int Quantity { get; set; }
    }
}
