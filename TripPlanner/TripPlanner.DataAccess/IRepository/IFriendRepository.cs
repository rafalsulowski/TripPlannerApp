using TripPlanner.Models.Models;
using TripPlanner.Models.Models.UserModels;

namespace TripPlanner.DataAccess.IRepository
{
    public interface IFriendRepository : IRepository<Friend>
    {
        Task<RepositoryResponse<bool>> Update(Friend post);
    }
}
