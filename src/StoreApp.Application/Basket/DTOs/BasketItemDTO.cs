using System;
using System.Collections.Generic;
using System.Text;

namespace StoreApp.Application.Basket.DTOs
{
    //  فقط برای انتقال داده استفاده می‌شود
    // این DTO برای انتقال اطلاعات یک آیتم سبد خرید استفاده می‌شود.
    // هدف  جدا کردن مدل خروجی Application از Entity مربوط به Domain است.
    public   class BasketItemDTO
    {
        public long Id { get; set; }
      
        public long ProductId { get; set; }

        public int Quantity { get; set; }

        public decimal UnitPrice { get; set; }
    }
}
