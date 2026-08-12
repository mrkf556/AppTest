using System;
using System.Collections.Generic;
using System.Text;

namespace StoreApp.Application.Abstractions.CQRS
{
    public interface ICommandDispatcher
    {
        ///قرار دادمشترک handler که توسط dispacher شناسایی بشوند
        ///خروجی متد میتواند بر اساس تایپ ورود ICommand و خروجی متد میتواند بر اساس تایپ خروجی  باشد
        Task<TResponse> Send<TResponse>(ICommand<TResponse> command,CancellationToken cancellationToken = default);
    }
}
