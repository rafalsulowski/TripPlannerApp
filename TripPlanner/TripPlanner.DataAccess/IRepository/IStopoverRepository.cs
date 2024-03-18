using TripPlanner.Models;
using TripPlanner.Models.Models;
using TripPlanner.Models.Models.RouteModels;

namespace TripPlanner.DataAccess.IRepository
{
    public interface IStopoverRepository : IRepository<Stopover>
    {
        Task<RepositoryResponse<bool>> Update(Stopover post);
    }
}
