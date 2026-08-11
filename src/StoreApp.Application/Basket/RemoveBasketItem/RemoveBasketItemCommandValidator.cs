using FluentValidation;
using StoreApp.Application.Abstractions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace StoreApp.Application.Basket.RemoveBasketItem
{
    //اعتبارسنجی به‌صورت خودکار
    public   class RemoveBasketItemCommandValidator : AbstractValidator<RemoveBasketItemCommand>, IScopedDependency
    {


        ///قبل از ورود درخواست به هندلر
        public RemoveBasketItemCommandValidator()
        {


            RuleFor(x => x.UserId)
                .GreaterThan(0)
                .WithMessage("شناسه کاربر باید بزرگ‌تر از صفر باشد.");

            RuleFor(x => x.ProductId)
                .GreaterThan(0)
                .WithMessage("شناسه کالا باید بزرگ‌تر از صفر باشد.");
        }
    }
}
