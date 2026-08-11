using System;
using System.Collections.Generic;
using System.Text;

namespace StoreApp.Application.Abstractions.Contracts
{
    // این Interface قرارداد عمومی دسترسی به داده‌ها را تعریف می‌کند.
    // هدف آن جدا کردن منطق Application از جزئیات پیاده‌سازی دیتابیس است.
    public interface IRepository<TEntity>
      where TEntity : class
    {
        Task<TEntity?> GetByIdAsync(
            long id,
            CancellationToken cancellationToken = default);

        Task AddAsync(
            TEntity entity,
            CancellationToken cancellationToken = default);

        void Update(TEntity entity);

        void Delete(TEntity entity);
    }
}
