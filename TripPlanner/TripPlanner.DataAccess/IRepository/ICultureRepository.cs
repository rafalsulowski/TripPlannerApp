using TripPlanner.Models;
using TripPlanner.Models.Models;
using TripPlanner.Models.Models.CultureModels;

namespace TripPlanner.DataAccess.IRepository
{
    public interface ICultureRepository : IRepository<Culture>
    {
        Task<RepositoryResponse<bool>> Update(Culture post);
    }
}
