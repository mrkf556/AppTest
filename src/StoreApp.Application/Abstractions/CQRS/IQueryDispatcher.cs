using System;
using System.Collections.Generic;
using System.Text;

namespace StoreApp.Application.Abstractions.CQRS
{
    public interface IQueryDispatcher
    {
        Task<TResponse> Send<TResponse>(IQuery<TResponse> query,CancellationToken cancellationToken = default);
    }
}
