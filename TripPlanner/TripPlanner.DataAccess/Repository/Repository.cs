using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using TripPlanner.DataAccess.IRepository;
using TripPlanner.Models;
using TripPlanner.Models.Models;
using TripPlanner.Models.Models.TourModels;

namespace TripPlanner.DataAccess.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationDbContext _db;
        internal DbSet<T> dbSet;
        public Repository(ApplicationDbContext db)
        {
            _db=db;
            this.dbSet=_db.Set<T>();
        }

        public T Add(T entity)
        {
            return dbSet.Add(entity).Entity;
        }

        public void AddRange(List<T> entity)
        {
            dbSet.AddRange(entity);
        }

        public async Task<RepositoryResponse<List<T>>> GetAll(Expression<Func<T, bool>>? filter = null, string? includeProperties = null)
        {
            IQueryable<T> query = dbSet;
            if(filter != null)
            {
                query = query.Where(filter);
            }
            if (includeProperties != null)
            {
                foreach (var property in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(property);
                }
            }
            var data = await query.AsNoTracking().ToListAsync();
            return new RepositoryResponse<List<T>> { Data = data };
        }

        public async Task<RepositoryResponse<T>> GetFirstOrDefault(Expression<Func<T, bool>> filter, string? includeProperties = null)
        {
            IQueryable<T> query = dbSet;
            query = query.Where(filter);
            if (includeProperties != null)
            {
                foreach (var property in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(property);
                }
            }
            var data = await query.AsNoTracking().SingleOrDefaultAsync();
            return new RepositoryResponse<T> { Data = data };
        }

        public void Remove(T entity)
        {
            if(entity != null)
            {
                dbSet.Remove(entity);
            }
        }

        public void RemoveRange(IEnumerable<T> entities)
        {
            if (entities != null)
            {
                foreach (T entity in entities)
                {
                    dbSet.Attach(entity);
                }
                dbSet.RemoveRange(entities);
            }
        }

        public async Task<RepositoryResponse<bool>> SaveChangesAsync()
        {
            try
            {
                await _db.SaveChangesAsync();
            } catch (Exception e )
            {
                return new RepositoryResponse<bool> { 
                    Success = false,
                    Data = false,
                    Message = e.Message
                };
            }
            
            return new RepositoryResponse<bool> { Data = true };
        }
    }
}
