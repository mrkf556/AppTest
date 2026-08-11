using Microsoft.EntityFrameworkCore;
using StoreApp.Application.Abstractions.Contracts;
using System;
using System.Collections.Generic;
using System.Text;

namespace StoreApp.Infrastructure.Repositories
{
    ///وجود این کلاس جهت اینکه با استفاده ان رویه دیتا موجود در اسنپ شات تغییرات را اعمال میکنیم اما جهت اعمال رویه دیتابیس استفادع از unitOfWork
    public class Repository<TEntity> : IRepository<TEntity>
     where TEntity : class
    {
        protected readonly DbContext Context;
        protected readonly DbSet<TEntity> DbSet;

        public Repository(DbContext context)
        {
            Context = context;
            DbSet = context.Set<TEntity>();
        }

        public virtual async Task<TEntity?> GetByIdAsync( long id, CancellationToken cancellationToken = default)
        {
            return await DbSet.FindAsync(
                new object[] { id },
                cancellationToken);
        }

        public virtual async Task AddAsync(  TEntity entity,  CancellationToken cancellationToken = default)
        {
            await DbSet.AddAsync(entity, cancellationToken);
        }

        public virtual void Update(TEntity entity)
        {
            DbSet.Update(entity);
        }

        public virtual void Delete(TEntity entity)
        {
            DbSet.Remove(entity);
        }
    }
}
