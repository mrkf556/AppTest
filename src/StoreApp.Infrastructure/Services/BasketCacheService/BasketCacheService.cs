using System.Text.Json;
using StackExchange.Redis;
using StoreApp.Application.Abstractions.DependencyInjection;
using StoreApp.Application.Abstractions.Redis;
using StoreApp.Application.Basket.DTOs;

namespace StoreApp.Infrastructure.Services.BasketCacheService;

public class BasketCacheService : IBasketCacheService, IScopedDependency
{

    /// <summary>
    /// handler نباید در قدم اول مستقیماً به repository برود
    /// </summary>
    private readonly IConnectionMultiplexer _redis;

    public BasketCacheService(IConnectionMultiplexer redis)
    {
        _redis = redis;
    }

    public async Task<BasketDTO?> GetAsync(long userId,CancellationToken cancellationToken = default)
    {
        var db = _redis.GetDatabase();

        var key = $"basket:{userId}";
        var value = await db.StringGetAsync(key);

        if (value.IsNullOrEmpty)
            return null;

        return JsonSerializer.Deserialize<BasketDTO>(value.ToString());
    }

    public async Task SetAsync(long userId,BasketDTO basket,TimeSpan expiration,CancellationToken cancellationToken = default)
    {
        var db = _redis.GetDatabase();

        var key = $"basket:{userId}";

        var value = JsonSerializer.Serialize(basket);

        await db.StringSetAsync(key,value,expiration);
    }

    public async Task RemoveAsync(long userId,CancellationToken cancellationToken = default)
    {
        var db = _redis.GetDatabase();

        var key = $"basket:{userId}";

        await db.KeyDeleteAsync(key);
    }
}