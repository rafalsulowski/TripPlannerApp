using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using TripPlanner.Models;
using TripPlanner.Models.DTO.TourDTOs;
using TripPlanner.Models.DTO.UserDTOs;
using TripPlanner.Models.Models;
using TripPlanner.Models.Models.UserModels;

namespace TripPlanner.Services.UserService
{
    public interface IUserService
    {
        Task<RepositoryResponse<List<User>>> GetUsersAsync(Expression<Func<User, bool>>? filter = null, string? includeProperties = null);
        Task<RepositoryResponse<User>> GetUserAsync(Expression<Func<User, bool>> filter, string? includeProperties = null);
        Task<RepositoryResponse<Friend>> GetFriendAsync(Expression<Func<Friend, bool>> filter, string? includeProperties = null);
        Task<RepositoryResponse<List<ExtendFriendDTO>>> GetFriends(int userId, int tourId = -1);
        Task<RepositoryResponse<List<ExtendUserDTO>>> ExtendUserDto(int userId);
        Task<RepositoryResponse<bool>> CreateUser(User user);
        Task<RepositoryResponse<bool>> UpdateUser(User user);
        Task<RepositoryResponse<bool>> DeleteUser(User user);
    }
}
