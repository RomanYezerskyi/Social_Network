using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using DataAccess.Data;
using DataAccess.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Social_Network;

namespace DataAccess
{
    public class BaseRepository<TEntity> : IAsyncRepository<TEntity> where TEntity : class
    {
        private ApplicationDbContext _context;
        private DbSet<TEntity> dbSet;
        public BaseRepository(ApplicationDbContext context)
        {
            this._context = context;
            dbSet = context.Set<TEntity>();
        }
        public async Task<IEnumerable<TEntity>> GetAsync(Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> includes = null, 
            Expression<Func<TEntity, bool>> filter = null)
        {
            IQueryable<TEntity> queryable = _context.Set<TEntity>();
            if (includes != null)
            {
                queryable = includes(queryable);
            }

            if (filter != null)
            {
                queryable = queryable.Where(filter);
            }
            if (orderBy != null)
            {
                return await orderBy(queryable).ToListAsync();
            }
            
            return await queryable.ToListAsync();
        }
        public async Task<IEnumerable<TEntity>> GetAsync(int? page, int limit, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> includes = null,
            Expression<Func<TEntity, bool>> filter = null)
        {
            IQueryable<TEntity> queryable = _context.Set<TEntity>();
            if (includes != null)
            {
                queryable = includes(queryable);
            }

            if (filter != null)
            {
                queryable = queryable.Where(filter);
            }
            if (orderBy != null)
            {
                return await PaginatedListGeneric<TEntity>.CreateAsync(orderBy(queryable).AsNoTracking(), page ?? 1, limit);
            }

            return await PaginatedListGeneric<TEntity>.CreateAsync(queryable.AsNoTracking(), page ?? 1, limit);
        }
        public async Task<TEntity> GetAsync(Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>> includes = null, Expression<Func<TEntity, bool>> filter = null)
        {
            IQueryable<TEntity> queryable = _context.Set<TEntity>();
            if (includes != null)
            {
                queryable = includes(queryable);
            }
            
            if (filter != null)
            {
                queryable = queryable.Where(filter);
            }

            return await queryable.FirstOrDefaultAsync();
        }
        public async Task<int> GetCountAsync(Expression<Func<TEntity, bool>> filter = null)
        {
            IQueryable<TEntity> queryable = _context.Set<TEntity>();
            if (filter != null)
            {
                return await queryable.Where(filter).CountAsync();
            }
            return await queryable.CountAsync();
        }
        public async Task InsertAsync(TEntity entity)
        {
            await _context.Set<TEntity>().AddAsync(entity);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(TEntity entityToUpdate)
        {
            _context.Update(entityToUpdate);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteAsync(TEntity entityToDelete)
        {
            _context.Set<TEntity>().Remove(entityToDelete);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteAsync(IEnumerable<TEntity> entityToDelete)
        {
            _context.Set<TEntity>().RemoveRange(entityToDelete);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteAsync(object id)
        {
            TEntity entityToDelete = await _context.Set<TEntity>().FindAsync(id);
            await DeleteAsync(entityToDelete);
            await _context.SaveChangesAsync();
        }
    }
}
