using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using StoreApp.Application.Abstractions.DependencyInjection;
using StoreApp.Application.Abstractions.Results;
using System;
using System.Collections.Generic;
using System.Text;

namespace StoreApp.Application.Abstractions.Pipeline
{
    public   class ValidationBehavior<TRequest, TResponse>: IPipelineBehavior<TRequest, TResponse>, IScopedDependency where TRequest : class where TResponse : ServiceResult
    {
        private readonly IServiceProvider _serviceProvider;

        public ValidationBehavior(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task<TResponse> Handle(TRequest request,CancellationToken cancellationToken,Func<Task<TResponse>> next)
        {

            ///سرویس هایی validator را دارند
            var validator =_serviceProvider.GetService<IValidator<TRequest>>();

            // اگر برای Command مربوطه Validator وجود نداشت،
            // درخواست مستقیماً به مرحله بعد می‌رود.
            if (validator is null)
            {
                return await next();
            }

            var validationResult =await validator.ValidateAsync(request, cancellationToken);

            // اگر اعتبارسنجی موفق بود، مرحله بعد اجرا می‌شود.
            if (validationResult.IsValid)
            {
                return await next();
            }

            var errorMessage = string.Join(" | ",validationResult.Errors.Select(x => x.ErrorMessage));

            return (TResponse)(object)ServiceResult.Failure(errorMessage);
        }
    }
}
