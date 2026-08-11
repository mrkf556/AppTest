using System;
using System.Collections.Generic;
using System.Text;

namespace StoreApp.Application.Abstractions.CQRS
{
    public interface IQueryHandler<TQuery, TResponse>
       where TQuery : IQuery<TResponse>
    {
        Task<TResponse> Handle(
            TQuery query,
            CancellationToken cancellationToken = default);
    }
}
