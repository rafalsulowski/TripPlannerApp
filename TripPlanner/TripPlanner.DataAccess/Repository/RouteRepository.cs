using Microsoft.EntityFrameworkCore;
using TripPlanner.DataAccess.IRepository;
using TripPlanner.DataAccess.Repository;
using TripPlanner.Models;
using TripPlanner.Models.Models;
using TripPlanner.Models.Models.RouteModels;

namespace TripPlanner.DataAccess.Repository
{
    public class RouteRepository : Repository<Route>, IRouteRepository
    {
        private ApplicationDbContext _context;
        public RouteRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<RepositoryResponse<bool>> Update(Route post)
        {
            var routeDB = _context.Routes.FirstOrDefault(u => u.Id == post.Id);
            if (routeDB == null)
            {
                return new RepositoryResponse<bool>
                {
                    Success = false,
                    Data = false,
                    Message = $"Nie istnieje trasa o id = {routeDB.Id}"
                };
            }

            _context.Entry(routeDB).State = EntityState.Detached;
            _context.Routes.Attach(post);
            _context.Entry(post).State = EntityState.Modified;
            return new RepositoryResponse<bool> { Data = true };
        }

        public async Task<RepositoryResponse<bool>> AddStopoverToRoute(Stopover Stopover)
        {
            var StopoverDB = _context.Stopovers.FirstOrDefault(u => u.RouteId == Stopover.RouteId && u.Name == Stopover.Name);
            if (StopoverDB == null)
            {
                _context.Stopovers.Add(Stopover);
            }
            else
            {
                _context.Stopovers.Attach(Stopover);
                _context.Entry(Stopover).State = EntityState.Modified;
            }
            return new RepositoryResponse<bool> { Data = true };
        }

        public async Task<RepositoryResponse<bool>> UpdateStopover(Stopover Stopover)
        {
            var StopoverDB = _context.Stopovers.FirstOrDefault(u => u.RouteId == Stopover.RouteId && u.Id == Stopover.Id);
            if (StopoverDB == null)
            {
                return new RepositoryResponse<bool>
                {
                    Success = false,
                    Data = false,
                    Message = "Nie istnieje taki postoj"
                };
            }
            _context.Entry(StopoverDB).State = EntityState.Detached;
            _context.Stopovers.Attach(Stopover);
            _context.Entry(Stopover).State = EntityState.Modified;
            return new RepositoryResponse<bool> { Data = true };
        }

        public async Task<RepositoryResponse<bool>> DeleteStopoverFromRoute(Stopover Stopover)
        {
            var res = _context.Stopovers.FirstOrDefault(u => u.Id == Stopover.Id && u.RouteId == Stopover.RouteId);
            if (res != null)
            {
                _context.Stopovers.Remove(res);
            }
            return new RepositoryResponse<bool> { Data = true };
        }
    }
}
