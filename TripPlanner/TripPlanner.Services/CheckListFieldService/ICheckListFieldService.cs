using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using TripPlanner.Models;
using TripPlanner.Models.Models;
using TripPlanner.Models.Models.CheckListModels;

namespace TripPlanner.Services.CheckListFieldService
{
    public interface ICheckListFieldService
    {
        Task<RepositoryResponse<List<CheckListField>>> GetCheckListFieldsAsync(Expression<Func<CheckListField, bool>>? filter = null, string? includeProperties = null);
        Task<RepositoryResponse<CheckListField>> GetCheckListFieldAsync(Expression<Func<CheckListField, bool>> filter, string? includeProperties = null);
        Task<RepositoryResponse<bool>> CreateCheckListField(CheckListField Bill);
        Task<RepositoryResponse<bool>> UpdateCheckListField(CheckListField Bill);
        Task<RepositoryResponse<bool>> DeleteCheckListField(CheckListField Bill);
    }
}
