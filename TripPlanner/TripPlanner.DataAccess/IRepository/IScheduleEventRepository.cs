using TripPlanner.DataAccess.Repository;
using TripPlanner.Models.Models;
using TripPlanner.Models.Models.ScheduleModels;

namespace TripPlanner.DataAccess.IRepository
{
    public interface IScheduleEventRepository : IRepository<ScheduleEvent>
    {
        Task<RepositoryResponse<bool>> Update(ScheduleEvent post);
    }
}
