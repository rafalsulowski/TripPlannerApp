using System.Linq.Expressions;
using TripPlanner.Models.Models;
using TripPlanner.Models.Models.UserModels;

namespace TripPlanner.Services.FriendService
{
    public interface IFriendService
    {
        Task<RepositoryResponse<List<Friend>>> GetFriendsAsync(Expression<Func<Friend, bool>>? filter = null, string? includeProperties = null);
        Task<RepositoryResponse<Friend>> GetFriendAsync(Expression<Func<Friend, bool>> filter, string? includeProperties = null);
        Task<RepositoryResponse<bool>> CreateFriend(Friend Bill);
        Task<RepositoryResponse<bool>> UpdateFriend(Friend Bill);
        Task<RepositoryResponse<bool>> DeleteFriend(Friend Bill);
    }
}
