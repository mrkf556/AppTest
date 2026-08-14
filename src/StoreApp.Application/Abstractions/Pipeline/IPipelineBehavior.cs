using System;
using System.Collections.Generic;
using System.Text;

namespace StoreApp.Application.Abstractions.Pipeline
{
    //اگر بخواهیم یک behavior بسازم باید از Ipipline ارث بری کند
    public interface IPipelineBehavior<TRequest, TResponse>
    {
         Task<TResponse> Handle(TRequest request,CancellationToken cancellationToken,Func<Task<TResponse>> next);
    }
}
