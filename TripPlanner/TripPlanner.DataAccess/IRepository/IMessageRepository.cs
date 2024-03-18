using TripPlanner.Models;
using TripPlanner.Models.Models;
using TripPlanner.Models.Models.MessageModels;

namespace TripPlanner.DataAccess.IRepository
{
    public interface IMessageRepository : IRepository<Message>
    {
        Task<RepositoryResponse<bool>> Update(Message post);
    }
}
