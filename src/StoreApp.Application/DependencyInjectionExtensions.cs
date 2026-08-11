using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace StoreApp.Application.Abstractions.DependencyInjection;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddMarkerDependencies(
        this IServiceCollection services,
        Assembly assembly)
    {
        var types = assembly
            .GetTypes()
            .Where(x =>
                x.IsClass &&
                !x.IsAbstract)
            .ToList();

        foreach (var implementationType in types)
        {
            var interfaces = implementationType.GetInterfaces();

            var lifetime =
                interfaces.Contains(typeof(IScopedDependency))
                    ? ServiceLifetime.Scoped
                    : interfaces.Contains(typeof(ITransientDependency))
                        ? ServiceLifetime.Transient
                        : interfaces.Contains(typeof(ISingletonDependency))
                            ? ServiceLifetime.Singleton
                            : (ServiceLifetime?)null;

            if (lifetime is null)
                continue;

            RegisterInterfaces(
                services,
                implementationType,
                interfaces,
                lifetime.Value);
        }
      
        return services;
    }

    private static void RegisterInterfaces(
        IServiceCollection services,
        Type implementationType,
        IEnumerable<Type> interfaces,
        ServiceLifetime lifetime)
    {
        foreach (var serviceType in interfaces)
        {
            if (serviceType == typeof(IScopedDependency) ||
                serviceType == typeof(ITransientDependency) ||
                serviceType == typeof(ISingletonDependency))
            {
                continue;
            }

            // Open Generic
            if (implementationType.IsGenericTypeDefinition &&
                serviceType.IsGenericType)
            {
                services.Add(
                    new ServiceDescriptor(
                        serviceType.GetGenericTypeDefinition(),
                        implementationType,
                        lifetime));

                continue;
            }

            // Normal class
            services.Add(
                new ServiceDescriptor(
                    serviceType,
                    implementationType,
                    lifetime));
        }
    }
}