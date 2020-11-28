using Repository.Entities;
using System;
using System.Data.Entity;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Repository
{
	public class Repository<T> where T : BaseEntity
	{
        protected readonly Context _dbContext;

        public Repository(Context dbContext)
        {
            _dbContext = dbContext;
        }

        public virtual async Task<T> GetByIdAsync(Guid id)
        {
            return await _dbContext.Set<T>().FindAsync(id);
        }

        public async Task<T> AddAsync(T entity)
        {
            _dbContext.Set<T>().Add(entity);
            await _dbContext.SaveChangesAsync();

            return entity;
        }

        public async Task UpdateAsync(T entity)
        {
            _dbContext.Entry(entity).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(T entity)
        {
            _dbContext.Set<T>().Remove(entity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate)
		{
            return await _dbContext.Set<T>().FirstOrDefaultAsync(predicate);
        }

    }
}
