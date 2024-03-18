using System.Linq.Expressions;
using TripPlanner.DataAccess.IRepository;
using TripPlanner.Models.Models;
using TripPlanner.Models.Models.CheckListModels;

namespace TripPlanner.Services.CheckListFieldService
{
    public class CheckListFieldService : ICheckListFieldService
    {
        private readonly ICheckListFieldRepository _CheckListFieldRepository;
        public CheckListFieldService(ICheckListFieldRepository CheckListFieldRepository)
        {
            _CheckListFieldRepository = CheckListFieldRepository;
        }

        public async Task<RepositoryResponse<bool>> CreateCheckListField(CheckListField CheckListField)
        {
            _CheckListFieldRepository.Add(CheckListField);
            var response = await _CheckListFieldRepository.SaveChangesAsync();
            return response;
        }

        public async Task<RepositoryResponse<bool>> DeleteCheckListField(CheckListField CheckListField)
        {
            _CheckListFieldRepository.Remove(CheckListField);
            var response = await _CheckListFieldRepository.SaveChangesAsync();
            return response;
        }

        public async Task<RepositoryResponse<CheckListField>> GetCheckListFieldAsync(Expression<Func<CheckListField, bool>> filter, string? includeProperties = null)
        {
            var response = await _CheckListFieldRepository.GetFirstOrDefault(filter, includeProperties);
            return response;
        }

        public async Task<RepositoryResponse<List<CheckListField>>> GetCheckListFieldsAsync(Expression<Func<CheckListField, bool>>? filter = null, string? includeProperties = null)
        {
            var response = await _CheckListFieldRepository.GetAll(filter, includeProperties);
            return response;
        }

        public async Task<RepositoryResponse<bool>> UpdateCheckListField(CheckListField CheckListField)
        {
            var response = await _CheckListFieldRepository.Update(CheckListField);
            if(response.Success==false)
            {
                return response;
            }
            response = await _CheckListFieldRepository.SaveChangesAsync();
            return response;
        }
    }
}
