using StoreApp.Application.Abstractions.CQRS;
using StoreApp.Application.Abstractions.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace StoreApp.Application.Basket.UpdateBasketItemQuantity
{
    public   record UpdateBasketItemQuantityCommand( long UserId,   long ProductId, int NewQuantity)  : ICommand<ServiceResult>
    {

    }
}
