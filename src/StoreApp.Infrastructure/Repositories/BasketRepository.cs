using Microsoft.EntityFrameworkCore;
using StoreApp.Application.Abstractions.Contracts;
using StoreApp.Domain.Enitities;
using StoreApp.Domain.Enums;
using StoreApp.Infrastructure.Persistence;
using StoreApp.Application.Abstractions.DependencyInjection;
namespace StoreApp.Infrastructure.Repositories
{

    //فارغ از عملیات های کلی که نیاز هر مدل هست ما یک سری عملیات اختصاصی مخصوص ان مدل ایحاد میکنیم که در بقیه مدل ها مشترک نیست
    public class BasketRepository  : Repository<Basket>, IBasketRepository, IScopedDependency
    {



        private readonly BasketDbContext _context;

        public BasketRepository(BasketDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Basket?> GetActiveBasketByUserIdAsync(long userId,CancellationToken cancellationToken = default)
        {
            ////
            return await _context.Baskets
                ///eager include 
                //////یکی از انواع لود رابطه 
                .Include(b => b.Items)
                .FirstOrDefaultAsync(b => b.UserId == userId &&b.Status == BasketStatus.Active,cancellationToken);
        }
        public async Task<List<Basket>> GetExpiredBasketsAsync(DateTime expirationTime,CancellationToken cancellationToken = default)
        {
            // Basket یک Aggregate
            return await _context.Baskets.Include(b => b.Items).
                Where(b =>b.Status == BasketStatus.Active &&b.LastUpdatedAt.HasValue &&b.LastUpdatedAt.Value <= expirationTime).ToListAsync(cancellationToken);
        }
    }
}
