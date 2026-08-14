using Microsoft.Extensions.DependencyInjection;
using StoreApp.Application.Abstractions.CQRS;
using StoreApp.Application.Abstractions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace StoreApp.Infrastructure.Services.CQRS
{
    // مانند commandhandler، برای شناسایی و اجرای queryhandler
    // توسط querydispatcher استفاده می‌شود
    public   class QueryDispatcher : IQueryDispatcher, IScopedDependency
    {
        private readonly IServiceProvider _serviceProvider;

        public QueryDispatcher(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task<TResponse> Send<TResponse>(IQuery<TResponse> query,CancellationToken cancellationToken = default)
        {
            var queryType = query.GetType();

            var handlerType = typeof(IQueryHandler<,>).MakeGenericType(queryType, typeof(TResponse));

            var handler = _serviceProvider.GetRequiredService(handlerType);

            return await ((dynamic)handler).Handle((dynamic)query, cancellationToken);
        }
    }
}
