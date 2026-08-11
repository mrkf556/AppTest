using StoreApp.Application.Abstractions.Contracts;
using StoreApp.Application.Abstractions.CQRS;
using StoreApp.Application.Abstractions.DependencyInjection;
using StoreApp.Application.Abstractions.Redis;
using StoreApp.Application.Abstractions.Results;
using StoreApp.Infrastructure.Persistence;

namespace StoreApp.Application.Basket.AddItemToBasket
{
   
        public   class AddItemToBasketCommandHandler:ICommandHandler<AddItemToBasketCommand, ServiceResult>, IScopedDependency
          {
            private readonly IBasketRepository _basketRepository;
            private readonly IUnitOfWork _unitOfWork;
            private readonly IBasketCacheService _basketCacheService;
        public AddItemToBasketCommandHandler(IBasketRepository basketRepository,IUnitOfWork unitOfWork,IBasketCacheService basketCacheService)
        {
            _basketRepository = basketRepository;
            _unitOfWork = unitOfWork;
            _basketCacheService = basketCacheService;
        }

        public async Task<ServiceResult> Handle( AddItemToBasketCommand command, CancellationToken cancellationToken)
            {

            const decimal unitPrice = 1_000_000m;


            var basket = await _basketRepository.GetActiveBasketByUserIdAsync(command.UserId, cancellationToken);
 
            try
            {
                ///agregate root with basket
                basket.AddItem(command.Item.ProductId, command.Item.Quantity, unitPrice);
                ///دستحوش تغییر گردید دیتا قبلی ان حذف 
                await _basketCacheService.RemoveAsync(command.UserId,cancellationToken);

                return ServiceResult.Success();
            }
            catch (InvalidOperationException ex)
            {
                return ServiceResult.Failure(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return ServiceResult.Failure(ex.Message);
            }

        }
    }
    
}
