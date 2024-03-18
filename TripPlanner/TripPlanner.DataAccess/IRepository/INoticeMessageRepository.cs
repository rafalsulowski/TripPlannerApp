using TripPlanner.Models;
using TripPlanner.Models.Models;
using TripPlanner.Models.Models.MessageModels;

namespace TripPlanner.DataAccess.IRepository
{
    public interface INoticeMessageRepository : IRepository<NoticeMessage>
    {
        Task<RepositoryResponse<bool>> Update(NoticeMessage post);
    }
}
