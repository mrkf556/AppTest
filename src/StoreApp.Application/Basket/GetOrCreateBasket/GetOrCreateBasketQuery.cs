using StoreApp.Application.Abstractions.CQRS;
using StoreApp.Application.Abstractions.DTOs;
using StoreApp.Application.Abstractions.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace StoreApp.Application.Basket.GetOrCreateBasket
{
    //Query فقط داده‌ی ورودی
    public   record GetOrCreateBasketQuery(long UserId): IQuery<ServiceResult<BasketDTO>>
    {

    }
}
