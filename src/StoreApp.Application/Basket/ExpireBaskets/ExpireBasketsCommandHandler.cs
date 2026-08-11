using StoreApp.Application.Abstractions.Contracts;
using StoreApp.Application.Abstractions.CQRS;
using StoreApp.Application.Abstractions.DependencyInjection;
using StoreApp.Application.Abstractions.Redis;
using StoreApp.Application.Abstractions.Results;
using StoreApp.Application.Basket.UpdateBasketItemQuantity;
using StoreApp.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Text;

namespace StoreApp.Application.Basket.ExpireBaskets
{

    //Basketهای Active را که بیشتر از ۳۰ دقیقه بدون فعالیت بوده‌اند دریافت کند
    //روی هر Basket متد دامنه‌ی Expire() را صدا بزند
    public   class ExpireBasketsCommandHandler : ICommandHandler<ExpireBasketsCommand, ServiceResult>, IScopedDependency
    {
        private readonly IBasketRepository _basketRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBasketCacheService _basketCacheService;
        public ExpireBasketsCommandHandler(IBasketRepository basketRepository,IUnitOfWork unitOfWork,IBasketCacheService basketCacheService)
        {
            _basketRepository = basketRepository;
            _unitOfWork = unitOfWork;
            _basketCacheService = basketCacheService;
        }

        public async Task<ServiceResult> Handle(ExpireBasketsCommand command,CancellationToken cancellationToken)
        {
            var expirationTime = DateTime.UtcNow.AddMinutes(30);

            var baskets = await _basketRepository.GetExpiredBasketsAsync(expirationTime,cancellationToken);

            foreach (var basket in baskets)
            {
                ///به ازای هر سبد پیدا شده 
                basket.Expire();
                await _basketCacheService.RemoveAsync(basket.UserId,cancellationToken);
            }
            //طبق تسک هندلر فقط وظیفه انجام منطق خودش باشد عمل تغییرات در ترنز اکشن اتفاق بیافتد
          //  await _unitOfWork.SaveChangesAsync(cancellationToken);

            return ServiceResult.Success();
        }
    }
}
