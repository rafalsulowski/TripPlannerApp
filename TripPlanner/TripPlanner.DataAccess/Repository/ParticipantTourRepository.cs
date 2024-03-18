using Microsoft.EntityFrameworkCore;
using TripPlanner.DataAccess.IRepository;
using TripPlanner.DataAccess.Repository;
using TripPlanner.Models;
using TripPlanner.Models.Models;
using TripPlanner.Models.Models.TourModels;

namespace TripPlanner.DataAccess.Repository
{
    public class ParticipantTourRepository : Repository<ParticipantTour>, IParticipantTourRepository
    {
        private ApplicationDbContext _context;
        public ParticipantTourRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<RepositoryResponse<bool>> Update(ParticipantTour post)
        {
            var postDB = await GetFirstOrDefault(u => u.TourId == post.TourId && u.UserId == post.UserId);
            var res = postDB.Data;
            if (postDB == null)
            {
                return new RepositoryResponse<bool>
                {
                    Success = false,
                    Data = false,
                    Message = "ParticipantTour with this Id was not found."
                };
            }
            _context.Entry(res).State = EntityState.Detached;
            _context.ParticipantTours.Attach(post);
            _context.Entry(post).State = EntityState.Modified;
            return new RepositoryResponse<bool> { Data = true };
        }
    }
}
