using System;
using System.Collections.Generic;
using System.Text;

namespace StoreApp.Domain.Enums
{
    // این Enum وضعیت فعلی سبد خرید را مشخص می‌کند.
    // سبد می‌تواند فعال یا کنسلل شده باشد.
    public enum BasketStatus
    {
        Active = 1,
        Expired = 2
    }
}
