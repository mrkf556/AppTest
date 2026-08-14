using StoreApp.Application.Abstractions.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace StoreApp.Application.Abstractions.Redis
{
    public interface IBasketCacheService
    {
        Task<BasketDTO?> GetAsync(long userId, CancellationToken cancellationToken= default);

        Task SetAsync(long userId,BasketDTO basket,TimeSpan expiration,CancellationToken cancellationToken =default);

        Task RemoveAsync(long userId,CancellationToken cancellationToken = default);
    }
}
