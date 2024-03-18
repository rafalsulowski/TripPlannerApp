using Microsoft.EntityFrameworkCore;
using TripPlanner.DataAccess.IRepository;
using TripPlanner.DataAccess.Repository;
using TripPlanner.Models;
using TripPlanner.Models.Models;
using TripPlanner.Models.Models.CultureModels;

namespace TripPlanner.DataAccess.Repository
{
    public class CultureAssistanceRepository : Repository<CultureAssistance>, ICultureAssistanceRepository
    {
        private ApplicationDbContext _context;
        public CultureAssistanceRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<RepositoryResponse<bool>> Update(CultureAssistance post)
        {
            var postDB = await GetFirstOrDefault(u => u.TourId == post.TourId && u.CultureId == post.CultureId);
            if (postDB == null)
            {
                return new RepositoryResponse<bool>
                {
                    Success = false,
                    Data = false,
                    Message = "CultureAssistance with this Id was not found."
                };
            }
            _context.CultureAssistances.Attach(post);
            _context.Entry(post).State = EntityState.Modified;
            return new RepositoryResponse<bool> { Data = true };
        }
    }
}
