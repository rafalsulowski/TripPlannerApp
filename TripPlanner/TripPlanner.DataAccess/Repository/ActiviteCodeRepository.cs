using Microsoft.EntityFrameworkCore;
using TripPlanner.DataAccess.IRepository;
using TripPlanner.Models.Models;
using TripPlanner.Models.Models.UserModels;

namespace TripPlanner.DataAccess.Repository
{
    public class ActiviteCodeRepository : Repository<ActiviteCode>, IActiviteCodeRepository
    {
        private ApplicationDbContext _context;
        public ActiviteCodeRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
    }
}
