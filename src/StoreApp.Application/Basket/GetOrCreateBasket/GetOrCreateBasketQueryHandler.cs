using StoreApp.Application.Abstractions.Contracts;
using StoreApp.Application.Abstractions.CQRS;
using StoreApp.Application.Abstractions.DependencyInjection;
using StoreApp.Application.Abstractions.DTOs;
using StoreApp.Application.Abstractions.Redis;
using StoreApp.Application.Abstractions.Results;
 using StoreApp.Infrastructure.Persistence;
using BasketEntity = StoreApp.Domain.Enitities.Basket;

namespace StoreApp.Application.Basket.GetOrCreateBasket
{
    ///روند درخواست ابتدا از کش هست مستقیم به دیتا بیس درخواست ارسال نمی شود
    public   class GetOrCreateBasketQueryHandler: IQueryHandler<GetOrCreateBasketQuery, ServiceResult<BasketDTO>>, IScopedDependency
    {
        private readonly IBasketRepository _basketRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBasketCacheService _basketCacheService;

        public GetOrCreateBasketQueryHandler(IBasketRepository basketRepository,IUnitOfWork unitOfWork,IBasketCacheService basketCacheService)
        {
            _basketRepository = basketRepository;
            _unitOfWork = unitOfWork;
            _basketCacheService = basketCacheService;
        }

        public async Task<ServiceResult<BasketDTO>> Handle(GetOrCreateBasketQuery query,CancellationToken cancellationToken)
        {
            var cachedBasket = await _basketCacheService.GetAsync(query.UserId, cancellationToken);
            ///بررسی دیتا موجود اگر وجود داشت دیگر به دیتابیس وارد نشودد
            if (cachedBasket is not null)
            {
                return ServiceResult<BasketDTO>.Success(cachedBasket);
            }

            var basket = await _basketRepository.GetActiveBasketByUserIdAsync(query.UserId,cancellationToken);

            if (basket is null)
            {
                basket = new BasketEntity(query.UserId);

                await _basketRepository.AddAsync(basket,cancellationToken);

                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }

            var basketDto = new BasketDTO
            {
                Id = basket.Id,
                UserId = basket.UserId,
                Status = (int)basket.Status,
                CreatedAt = basket.CreatedAt,
                LastUpdatedAt = basket.LastUpdatedAt,

                Items = basket.Items.Select(item => new BasketItemDTO
                    {
                        Id = item.Id,
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        UnitPrice = item.UnitPrice
                    })
                    .ToList()
            };

            await _basketCacheService.SetAsync(query.UserId,basketDto,TimeSpan.FromMinutes(5),cancellationToken);

            return ServiceResult<BasketDTO>.Success(basketDto);
        }
    }
}