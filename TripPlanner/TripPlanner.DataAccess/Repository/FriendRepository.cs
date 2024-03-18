using Microsoft.EntityFrameworkCore;
using TripPlanner.DataAccess.IRepository;
using TripPlanner.Models.Models;
using TripPlanner.Models.Models.UserModels;

namespace TripPlanner.DataAccess.Repository
{
    public class FriendRepository : Repository<Friend>, IFriendRepository
    {
        private ApplicationDbContext _context;
        public FriendRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<RepositoryResponse<bool>> Update(Friend post)
        {
            var FriendDB = _context.Friends.FirstOrDefault(u => u.Friend1Id == post.Friend1Id && u.Friend2Id == post.Friend2Id);
            if (FriendDB == null)
            {
                return new RepositoryResponse<bool>
                {
                    Success = false,
                    Data = false,
                    Message = $"Nie istnieje powiązanie znajomych o Friend1Id = {post.Friend1Id} oraz Friend2Id = {post.Friend2Id}"
                };
            }

            _context.Entry(FriendDB).State = EntityState.Detached;
            _context.Friends.Attach(post);
            _context.Entry(post).State = EntityState.Modified;
            return new RepositoryResponse<bool> { Data = true };
        }
    }
}
