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

namespace StoreApp.Application.Basket.ClearBasket
{
    public class ClearBasketCommandHandler : ICommandHandler<ClearBasketCommand, ServiceResult>, IScopedDependency
    {

        private readonly IBasketRepository _basketRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBasketCacheService _basketCacheService;
        public ClearBasketCommandHandler(IBasketRepository basketRepository,IUnitOfWork unitOfWork,IBasketCacheService basketCacheService)
        {
            _basketRepository = basketRepository;
            _unitOfWork = unitOfWork;
            _basketCacheService = basketCacheService;
        }


        public async Task<ServiceResult> Handle(ClearBasketCommand command,CancellationToken cancellationToken)
        {
            ///بسکت به همراه ایتم های ان را میگیریم
            var basket =await _basketRepository.GetActiveBasketByUserIdAsync(command.UserId,cancellationToken);
            if (basket == null) {
                return ServiceResult.Failure("سبد خرید برای کاربر یافت نشد");
            
            }

            ///در صورت وجود ایتم در سبد خرید بیزینس داخل بسکت ایتم های موجود ان را حذف میکند
            basket.Clear();
            ////دیتایی که قبلا لود شده به همراه تغییرات داده شد برای بروز رسانی دیتا ترک شده به ریپازیتوری داده میشود که دیتا از قبل لود شده را بروز کند
            _basketRepository.Update(basket);
            await _basketCacheService.RemoveAsync(command.UserId,cancellationToken);
            ///
            ///  await _unitOfWork.SaveChangesAsync(cancellationToken);
            return ServiceResult.Success();




        }
    }
}
