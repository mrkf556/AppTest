using Microsoft.EntityFrameworkCore;
using StoreApp.Domain.Enitities;
using StoreApp.Domain.Events.DomainEvent;

namespace StoreApp.Infrastructure.Persistence
{
    
    public class BasketDbContext : DbContext
    {
        public BasketDbContext(DbContextOptions<BasketDbContext> options)   : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.ApplyConfigurationsFromAssembly(
                typeof(BasketDbContext).Assembly);
 
            base.OnModelCreating(modelBuilder);
       
        
        }
        public DbSet<Basket> Baskets => Set<Basket>();

        public DbSet<BasketItem> BasketItems => Set<BasketItem>();
        //دی بی رویداد ها را یافت میکند
        public List<IDomainEvent> GetDomainEvents()
        {
            return ChangeTracker.Entries<Basket>().SelectMany(entry => entry.Entity.DomainEvents).ToList();
        }
    }
}
