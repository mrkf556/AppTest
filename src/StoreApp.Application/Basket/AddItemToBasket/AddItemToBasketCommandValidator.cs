using FluentValidation;
using StoreApp.Application.Abstractions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace StoreApp.Application.Basket.AddItemToBasket
{
    //قبل از رسیدن Command به Handler اجرا خواهد شد
    //        ///قبل از ورود درخواست به هندلر

    public   class AddItemToBasketCommandValidator: AbstractValidator<AddItemToBasketCommand>, IScopedDependency
    {
        public AddItemToBasketCommandValidator()
        {



            RuleFor(x => x.UserId)
                .GreaterThan(0)
                .WithMessage("شناسه کاربر باید بزرگ‌تر از صفر باشد.");

            RuleFor(x => x.Item.ProductId)
                .GreaterThan(0)
                .WithMessage("شناسه کالا باید بزرگ‌تر از صفر باشد.");



            RuleFor(x => x.Item.Quantity)
                .InclusiveBetween(1, 10)
                .WithMessage("تعداد کالا باید بین ۱ تا ۱۰ باشد.");
       
        
        }
   
    
    }
}
