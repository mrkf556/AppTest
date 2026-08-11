using System;
using System.Collections.Generic;
using System.Text;

namespace StoreApp.Application.Basket.DTOs
{
    //  فقط برای انتقال داده استفاده می‌شود
    public   class BasketDTO
    {
        // این DTO برای انتقال اطلاعات سبد خرید بین لایه Application و API استفاده می‌شود.
        // این کلاس فقط داده‌های مورد نیاز خروجی را نگهداری می‌کند
        public long Id { get; set; }

        public long UserId { get; set; }

        public int Status { get; set; }

        public List<BasketItemDTO> Items { get; set; } = new();

        public DateTime CreatedAt { get; set; }

        public DateTime? LastUpdatedAt { get; set; }
    }
}
