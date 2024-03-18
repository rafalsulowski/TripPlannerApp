using TripPlanner.Models.Models;
using TripPlanner.Models.Models.UserModels;

namespace TripPlanner.DataAccess.IRepository
{
    public interface INotificationRepository : IRepository<Notification>
    {
        Task<RepositoryResponse<bool>> Update(Notification post);
    }
}
