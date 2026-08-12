using Microsoft.Extensions.DependencyInjection;
using StoreApp.Application.Abstractions.CQRS;
using StoreApp.Application.Abstractions.DependencyInjection;
using StoreApp.Application.Abstractions.Pipeline;
using System;
using System.Collections.Generic;
using System.Text;



namespace StoreApp.Infrastructure.Services.CQRS
{
    /// <summary>
    /// /وظیفه این قسمت
    /// 1 Controller این را می‌فرستد
    /// 2AddItemToBasketCommand 
    /// 3 Dispatcher نوع واقعی Command را می‌گیرد
    /// 4 و به این می‌رسد ICommandHandler<AddItemToBasketCommand, ServiceResult>
    /// 5 GetRequiredService(handlerType) ->DI
    /// 6 AddItemToBasketCommandHandler پیدا میکند
    /// </summary>
    public   class CommandDispatcher : ICommandDispatcher, IScopedDependency
    {
        /// <summary>
        /// سرویس هایی که در di نوشته شده را دریافت کنر
        /// </summary>
        private readonly IServiceProvider _serviceProvider;

        public CommandDispatcher(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        
      
        public async Task<TResponse> Send<TResponse>(ICommand<TResponse> command,CancellationToken cancellationToken = default)
        {
            //سه خط کد زیر عمل زیر را انجام میده

            ///کنترلر کامند را به دیس پچر میدهد دیس پچر و بعدش تمام وابستگی ها را میگیرد و میره براساس اون وابستگی ها داده شده بررسی میکند که کدام هندلر این کامند را دارد  
 



            ///نوع واقعی Command را می‌گیرد
            var commandType = command.GetType();
            ///  Handler پیدا می‌شود
            var handlerType = typeof(ICommandHandler<,>).MakeGenericType(commandType,typeof(TResponse));
            //DI آن Handler را پیدا می‌کند
            var handler = _serviceProvider.GetRequiredService(handlerType);
            
            ///با کد های بالا ما هندل رو پیدا  کردیم و متد هندلر را میگیریم
            ///به صورت دله گیت تعریف کردیم که بعدا اجرا کنیم 
            ////دلیل داینامیک بودن این هست که چون ران تایم هست در زمان کامپیال نوع ان مشخص نیست
            Func<Task<TResponse>> handlerDelegate = () =>((dynamic)handler).Handle((dynamic)command, cancellationToken);




            //Behaviorها را از DI می‌گیرد
            ///تمام behavior ها را که منطبق با کامند مورد نظر هست با میگیرد
            ///
            ////نکته وابستگی پایپ لاین رو جنریک تعریف شده از طرق کد زیر اون رو براش مشخص میکنیم کامند را داریم و بقیه را به ان میدهیم
            var behaviorType = typeof(IPipelineBehavior<,>).MakeGenericType(commandType,typeof(TResponse));
            var behaviors = _serviceProvider.GetServices(behaviorType).Cast<dynamic>().ToList();
            foreach (var behavior in behaviors)
            {
                Console.WriteLine(
                    $"EXECUTION BEHAVIOR => {behavior.GetType().FullName}");
            }
            Func<Task<TResponse>> pipeline = handlerDelegate;


            //trans و valid و handler را به هم وصل می‌کند
            foreach (var behavior in behaviors.AsEnumerable().Reverse())
            {
                //نگگه داری اجرا قبلی
                var next = pipeline;

                pipeline = () =>behavior.Handle((dynamic)command,cancellationToken,next);
            }

            return await pipeline();
        }
    }
}
