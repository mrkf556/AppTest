using StoreApp.Application.Abstractions.CQRS;
using StoreApp.Application.Abstractions.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace StoreApp.Application.Basket.ExpireBaskets
{
    public class ExpireBasketsCommand : ICommand<ServiceResult>
    {
    }
}
