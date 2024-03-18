using TripPlanner.Models;
using TripPlanner.Models.Models;
using TripPlanner.Models.Models.TourModels;
using TripPlanner.Models.Models.UserModels;

namespace TripPlanner.DataAccess.IRepository
{
    public interface IUserRepository : IRepository<User>
    {
        Task<RepositoryResponse<bool>> Update(User post);
        Task<RepositoryResponse<bool>> AddFriend(Friend Contribute);
        Task<RepositoryResponse<bool>> DeleteFriend(Friend Contribute);
    }
}
