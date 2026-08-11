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

namespace StoreApp.Application.Basket.RemoveBasketItem
{
    public   class RemoveBasketItemCommandHandler : ICommandHandler<RemoveBasketItemCommand, ServiceResult>, IScopedDependency
    {
        private readonly IBasketRepository _basketRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBasketCacheService _basketCacheService;
        public RemoveBasketItemCommandHandler(IBasketRepository basketRepository,IUnitOfWork unitOfWork,IBasketCacheService basketCacheService)
        {
            _basketRepository = basketRepository;
            _unitOfWork = unitOfWork;
            _basketCacheService = basketCacheService;
        }

        public async Task<ServiceResult> Handle(  RemoveBasketItemCommand command,  CancellationToken cancellationToken)
        {


            var basket = await _basketRepository
                ///بررسی سبدی هست یا ن 
                .GetActiveBasketByUserIdAsync( command.UserId,  cancellationToken);
             
            if (basket is null)
            {
                return ServiceResult.Failure(
                    "سبد خرید فعالی برای کاربر پیدا نشد.");
            }
             
            var itemExists = basket.Items.Any(x => x.ProductId == command.ProductId);
            
            if (!itemExists)
            {
                return ServiceResult.Failure("محصول مورد نظر در سبد خرید وجود ندارد.");
            }
             basket.RemoveItem(command.ProductId);

            _basketRepository.Update(basket); await _basketCacheService.RemoveAsync(command.UserId, cancellationToken);
            await _basketCacheService.RemoveAsync(command.UserId,cancellationToken);
            //// تغییرات اعمال شده را در دیتابیس اعمال میکنیم
            // await _unitOfWork.SaveChangesAsync(  cancellationToken);

            return ServiceResult.Success();
        }
    }
}
