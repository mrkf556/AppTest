using StoreApp.Application.Abstractions.CQRS;
using StoreApp.Application.Abstractions.Results;
using StoreApp.Application.Basket.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace StoreApp.Application.Basket.AddItemToBasket
{
    public   record AddItemToBasketCommand(long UserId,AddBasketItemDTO Item): ICommand<ServiceResult>
    {

    }
}
