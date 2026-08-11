using System;
using System.Collections.Generic;
using System.Text;

namespace StoreApp.Application.Abstractions.CQRS
{
    public interface ICommandHandler<TCommand, TResponse>where TCommand : ICommand<TResponse>
    {
        Task<TResponse> Handle(TCommand command,CancellationToken cancellationToken =default);
    }
}
