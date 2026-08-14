using StoreApp.Application.Abstractions.CQRS;
using StoreApp.Application.Abstractions.DTOs;
using StoreApp.Application.Abstractions.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace StoreApp.Application.Basket.AddItemToBasket
{
    public   record AddItemToBasketCommand(long UserId,AddBasketItemDTO Item): ICommand<ServiceResult>
    {

    }
}
