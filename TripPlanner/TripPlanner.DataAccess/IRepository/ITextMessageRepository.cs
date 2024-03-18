using TripPlanner.Models;
using TripPlanner.Models.Models;
using TripPlanner.Models.Models.MessageModels;

namespace TripPlanner.DataAccess.IRepository
{
    public interface ITextMessageRepository : IRepository<TextMessage>
    {
        Task<RepositoryResponse<bool>> Update(TextMessage post);
    }
}
