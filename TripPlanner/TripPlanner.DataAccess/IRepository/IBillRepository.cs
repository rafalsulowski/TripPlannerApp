using TripPlanner.Models.Models;
using TripPlanner.Models.Models.BillModels;

namespace TripPlanner.DataAccess.IRepository
{
    public interface IBillRepository : IRepository<Bill>
    {
        Task<RepositoryResponse<bool>> Update(Bill post);
    }
}
