using FluentValidation;
using StoreApp.Application.Abstractions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace StoreApp.Application.Basket.UpdateBasketItemQuantity
{
    public   class UpdateBasketItemQuantityCommandValidator: AbstractValidator<UpdateBasketItemQuantityCommand>,IScopedDependency
    {
        public UpdateBasketItemQuantityCommandValidator(  )
        {


            RuleFor(x => x.UserId)
                .GreaterThan(0)

                .WithMessage("شناسه کاربر باید بزرگ‌تر از صفر باشد.");
            /////

            RuleFor(x => x.ProductId)
                .GreaterThan(0)
                .WithMessage("شناسه کالا باید بزرگ‌تر از صفر باشد.");
            /////
            RuleFor(x => x.NewQuantity)
                .InclusiveBetween(1, 10)
                .WithMessage("تعداد جدید کالا باید بین ۱ تا ۱۰ باشد.");
      
        }
    }
}
