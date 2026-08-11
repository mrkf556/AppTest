using StoreApp.Application.Abstractions.Contracts;
using StoreApp.Application.Abstractions.CQRS;
using StoreApp.Application.Abstractions.DependencyInjection;
using StoreApp.Application.Abstractions.Redis;
using StoreApp.Application.Abstractions.Results;
using StoreApp.Infrastructure.Persistence;
using System;
using System.Collections.Generic;
using System.Text;

namespace StoreApp.Application.Basket.UpdateBasketItemQuantity
{
    public   class UpdateBasketItemQuantityCommandHandler : ICommandHandler<UpdateBasketItemQuantityCommand, ServiceResult>, IScopedDependency
    {
        private readonly IBasketRepository _basketRepository;

        private readonly IUnitOfWork _unitOfWork;
        private readonly IBasketCacheService _basketCacheService;
        public UpdateBasketItemQuantityCommandHandler(IBasketRepository basketRepository,IUnitOfWork unitOfWork,IBasketCacheService basketCacheService)
        {
            _basketRepository = basketRepository;
            _unitOfWork = unitOfWork;
            _basketCacheService = basketCacheService;
        }

        public async Task<ServiceResult> Handle( UpdateBasketItemQuantityCommand command, CancellationToken cancellationToken)
        {

            var basket = await _basketRepository.GetActiveBasketByUserIdAsync(command.UserId,cancellationToken);


            if (basket is null)
            {
                return ServiceResult.Failure(
                    " سبد خرید  فعالی برای   کاربر پیدا  نشد");
            }



            var item = basket.Items.FirstOrDefault(x => x.ProductId == command.ProductId);

            await _basketCacheService.RemoveAsync(command.UserId,cancellationToken);
            ///
            if (item is null)
            {
                return ServiceResult.Failure("  محصول مورد نظر در سبد خرید وجود ندارد");
            }

            var unitPrice = item.UnitPrice;

            basket.UpdateQuantity(command.ProductId,command.NewQuantity,unitPrice);

            _basketRepository.Update(basket);
            ///
           // await _unitOfWork.SaveChangesAsync(cancellationToken);

            return ServiceResult.Success();
        }
    }
}
