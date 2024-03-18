using TripPlanner.Models.Models;
using TripPlanner.Models.Models.CheckListModels;

namespace TripPlanner.DataAccess.IRepository
{
    public interface ICheckListFieldRepository : IRepository<CheckListField>
    {
        Task<RepositoryResponse<bool>> Update(CheckListField post);
    }
}
