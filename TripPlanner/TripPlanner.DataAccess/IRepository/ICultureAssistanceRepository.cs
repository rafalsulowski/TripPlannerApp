using TripPlanner.Models;
using TripPlanner.Models.Models;
using TripPlanner.Models.Models.CultureModels;

namespace TripPlanner.DataAccess.IRepository
{
    public interface ICultureAssistanceRepository : IRepository<CultureAssistance>
    {
        Task<RepositoryResponse<bool>> Update(CultureAssistance post);
    }
}
