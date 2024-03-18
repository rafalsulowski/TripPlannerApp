using Microsoft.EntityFrameworkCore;
using TripPlanner.DataAccess.IRepository;
using TripPlanner.DataAccess.Repository;
using TripPlanner.Models;
using TripPlanner.Models.Models;
using TripPlanner.Models.Models.CultureModels;

namespace TripPlanner.DataAccess.Repository
{
    public class CultureRepository : Repository<Culture>, ICultureRepository
    {
        private ApplicationDbContext _context;
        public CultureRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<RepositoryResponse<bool>> Update(Culture post)
        {
            var postDB = await GetFirstOrDefault(u => u.Id == post.Id);
            var res = postDB.Data;
            if (postDB.Data == null)
            {
                return new RepositoryResponse<bool>
                {
                    Success = false,
                    Data = false,
                    Message = $"Nie instnieje nota kulturowa o id = {post.Id}"
                };
            }
            
            _context.Entry(res).State = EntityState.Detached;
            _context.Cultures.Attach(post);
            _context.Entry(post).State = EntityState.Modified;
            return new RepositoryResponse<bool> { Data = true };
        }
    }
}
