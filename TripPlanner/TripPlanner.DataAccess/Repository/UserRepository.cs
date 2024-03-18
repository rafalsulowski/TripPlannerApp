using Microsoft.EntityFrameworkCore;
using TripPlanner.DataAccess.IRepository;
using TripPlanner.Models.Models;
using TripPlanner.Models.Models.TourModels;
using TripPlanner.Models.Models.UserModels;

namespace TripPlanner.DataAccess.Repository
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        private ApplicationDbContext _context;
        public UserRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<RepositoryResponse<bool>> Update(User post)
        {
            var postDB = await GetFirstOrDefault(u => u.Id == post.Id);
            if (postDB == null)
            {
                return new RepositoryResponse<bool>
                {
                    Success = false,
                    Data = false,
                    Message = "User with this Id was not found."
                };
            }
            _context.Users.Attach(post);
            _context.Entry(post).State = EntityState.Modified;
            return new RepositoryResponse<bool> { Data = true };
        }

        public async Task<RepositoryResponse<bool>> AddFriend(Friend Friend)
        {
            var FriendDB = _context.Friends.FirstOrDefault(u => u.Friend1Id == Friend.Friend1Id && u.Friend2Id == Friend.Friend2Id);
            if (FriendDB == null)
            {
                _context.Friends.Add(Friend);
            }
            else
            {
                _context.Friends.Attach(Friend);
                _context.Entry(Friend).State = EntityState.Modified;
            }
            return new RepositoryResponse<bool> { Data = true };
        }

        public async Task<RepositoryResponse<bool>> DeleteFriend(Friend Friend)
        {
            var res = _context.Friends.FirstOrDefault(u => u.Friend1Id == Friend.Friend1Id && u.Friend2Id== Friend.Friend2Id);
            if (res != null)
            {
                _context.Friends.Remove(res);
            }
            return new RepositoryResponse<bool> { Data = true };
        }

    }
}
