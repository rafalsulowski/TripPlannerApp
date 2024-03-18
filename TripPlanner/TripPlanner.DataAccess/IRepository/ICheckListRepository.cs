using TripPlanner.Models.Models;
using TripPlanner.Models.Models.CheckListModels;

namespace TripPlanner.DataAccess.IRepository
{
    public interface ICheckListRepository : IRepository<CheckList>
    {
        Task<RepositoryResponse<bool>> Update(CheckList post);
        Task<RepositoryResponse<bool>> AddFieldToCheckList(CheckListField CheckListField);
        Task<RepositoryResponse<bool>> UpdateCheckListField(CheckListField CheckListField);
        Task<RepositoryResponse<bool>> DeleteFieldFromCheckList(CheckListField CheckListField);
    }
}
