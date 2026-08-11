using StoreApp.Application.Abstractions.CQRS;
using StoreApp.Application.Abstractions.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace StoreApp.Application.Basket.RemoveBasketItem
{
    public   record RemoveBasketItemCommand(long UserId,long ProductId): ICommand<ServiceResult>
    {

    }
}
