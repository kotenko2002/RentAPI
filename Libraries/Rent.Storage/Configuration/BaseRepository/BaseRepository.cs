using Microsoft.EntityFrameworkCore;
using Rent.Entities;

namespace Rent.Storage.Configuration.BaseRepository
{
    public class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : BaseEntity
    {
        protected ApplicationDbContext _context;
        protected DbSet<TEntity> Sourse;

        public BaseRepository(ApplicationDbContext context)
        {
            _context = context;
            Sourse = context.Set<TEntity>();
        }

        public async Task AddAsync(TEntity entity)
        {
            await Sourse.AddAsync(entity);
        }

        public async Task AddRangeAsync(IEnumerable<TEntity> entities)
        {
            await Sourse.AddRangeAsync(entities);
        }

        public Task RemoveAsync(TEntity entity)
        {
            Sourse.Remove(entity);
            return Task.CompletedTask;
        }

        public Task RemoveRangeAsync(IEnumerable<TEntity> entities)
        {
            Sourse.RemoveRange(entities);
            return Task.CompletedTask;
        }

        public async Task<IEnumerable<TEntity>> FindAllAsync()
        {
            return await Sourse.ToListAsync();
        }

        public async Task<TEntity> FindAsync(int id)
        {
            return await Sourse.FindAsync(id);
        }
    }
}
