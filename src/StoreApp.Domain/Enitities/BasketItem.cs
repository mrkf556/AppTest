using System;
using System.Collections.Generic;
using System.Text;

namespace StoreApp.Domain.Enitities
{
    // این کلاس کالا در سبد خرید است.
    // اطلاعاتی مانند شناسه کالا، تعداد و قیمت واحد در زمان افزودن
    // در این Entity نگهداری می‌شود.
    //BasketItem مسئول وضعیت خودش است و تغییر Quantity از مسیر Aggregate یعنی Basket انجام می‌شود.
    public class BasketItem
    {
        public long Id { get; private set; }

        public long ProductId { get; private set; }

        public int Quantity { get; private set; }

        public decimal UnitPrice { get; private set; }

        private BasketItem()

        {




        }

        internal BasketItem( long productId,  int quantity, decimal unitPrice)
        {
            ProductId = productId;
            Quantity = quantity;
            UnitPrice = unitPrice;
        }



        internal void UpdateQuantity(int quantity, decimal unitPrice)
        {
            Quantity = quantity;
            UnitPrice = unitPrice;
        }
    }
}
