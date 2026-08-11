using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

using StoreApp.Application.Abstractions.DependencyInjection;
using StoreApp.Application.Abstractions.MessageBus;

using StoreApp.Application.Basket.Events;

using StoreApp.Domain.Events.Basket;
using StoreApp.Domain.Events.DomainEvent;
using StoreApp.Infrastructure.MessageBus;
using StoreApp.Infrastructure.Persistence;
using StoreApp.Infrastructure.Services.DomainEvents;
using StoreApp.Infrastructure.Services.DomainEvents.BasketExpiration;


namespace StoreApp.Infrastructure
{
    ///به دلیل اینکه معماری کلین هست ارتباظ ها از بیرون به داخل هست نیازمند کلاس زیر هستیم جهت به کار بردن در program cs
    public static class DependencyInjectionInfra
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services,IConfiguration configuration)
        {
            services.AddDbContext<BasketDbContext>(options =>
             options.UseSqlServer(
               configuration.GetConnectionString("DefaultConnection")));
  
        
            services.AddScoped<IDomainEventDispatcher, DomainEventDispatcher>();

//            services.AddScoped<IDomainEventHandler<BasketExpiredEvent>,BasketExpiredEventHandler>();
            services.AddScoped<IBasketEventPublisher, RabbitMqBasketEventPublisher>();
            services.AddScoped<IDomainEventHandler<BasketExpiredEvent>,BasketExpiredEventRabbitMqHandler>();
            services.AddHostedService<BasketExpirationBackgroundService>();
            services.AddScoped<IDomainEventHandler<BasketItemAddedEvent>,BasketItemAddedEventHandler>();
            ///
            /// services.AddScoped<IPipelineBehavior<AddItemToBasketCommand, ServiceResult>,ValidationBehavior < AddItemToBasketCommand, ServiceResult >> ();
            ///
            var redisConnectionString =configuration.GetConnectionString("Redis");

            services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(redisConnectionString!));

            services.AddMarkerDependencies(typeof(DependencyInjectionInfra).Assembly);

             return services;
        }
         
    }
}
