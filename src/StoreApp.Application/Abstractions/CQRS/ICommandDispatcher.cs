using System;
using System.Collections.Generic;
using System.Text;

namespace StoreApp.Application.Abstractions.CQRS
{
    public interface ICommandDispatcher
    {
        ///قرار دادمشترک handler که توسط dispacher شناسایی بشوند
        Task<TResponse> Send<TResponse>(ICommand<TResponse> command,CancellationToken cancellationToken = default);
    }
}
