using TripPlanner.Models.Models.RouteModels;
using TripPlanner.Models.Models;
using TripPlanner.Models.Models.ScheduleModels;

namespace TripPlanner.DataAccess.IRepository
{
    public interface IScheduleDayRepository : IRepository<ScheduleDay>
    {
        Task<RepositoryResponse<bool>> AddScheduleEvent(ScheduleEvent Stopover);
        Task<RepositoryResponse<bool>> UpdateScheduleEvent(ScheduleEvent Stopover);
        Task<RepositoryResponse<bool>> DeleteScheduleEvent(ScheduleEvent Stopover);
    }
}
