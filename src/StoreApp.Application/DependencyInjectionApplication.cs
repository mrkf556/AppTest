using Microsoft.Extensions.DependencyInjection;
using StoreApp.Application.Abstractions.DependencyInjection;
using StoreApp.Application.Abstractions.Pipeline;
using StoreApp.Application.Basket.AddItemToBasket;
using StoreApp.Application.Basket.ClearBasket;
using StoreApp.Application.Basket.ExpireBaskets;
using StoreApp.Application.Basket.GetOrCreateBasket;
using StoreApp.Application.Basket.RemoveBasketItem;
using StoreApp.Application.Basket.UpdateBasketItemQuantity;

namespace StoreApp.Application;

public static class DependencyInjectionApplication
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMarkerDependencies(typeof(AddItemToBasketCommandHandler).Assembly);
     

        return services;
    }
}
