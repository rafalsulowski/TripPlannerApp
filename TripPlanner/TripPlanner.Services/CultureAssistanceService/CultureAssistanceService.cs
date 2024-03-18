using System.Linq.Expressions;
using TripPlanner.DataAccess.IRepository;
using TripPlanner.Models.Models;
using TripPlanner.Models.Models.CultureModels;

namespace TripPlanner.Services.CultureAssistanceService
{
    public class CultureAssistanceService : ICultureAssistanceService
    {
        private readonly ICultureAssistanceRepository _CultureAssistanceRepository;
        public CultureAssistanceService(ICultureAssistanceRepository CultureAssistanceRepository)
        {
            _CultureAssistanceRepository = CultureAssistanceRepository;
        }

        public async Task<RepositoryResponse<bool>> CreateCultureAssistance(CultureAssistance CultureAssistance)
        {
            _CultureAssistanceRepository.Add(CultureAssistance);
            var response = await _CultureAssistanceRepository.SaveChangesAsync();
            return response;
        }

        public async Task<RepositoryResponse<bool>> DeleteCultureAssistance(CultureAssistance CultureAssistance)
        {
            _CultureAssistanceRepository.Remove(CultureAssistance);
            var response = await _CultureAssistanceRepository.SaveChangesAsync();
            return response;
        }

        public async Task<RepositoryResponse<CultureAssistance>> GetCultureAssistanceAsync(Expression<Func<CultureAssistance, bool>> filter, string? includeProperties = null)
        {
            var response = await _CultureAssistanceRepository.GetFirstOrDefault(filter, includeProperties);
            return response;
        }

        public async Task<RepositoryResponse<List<CultureAssistance>>> GetCultureAssistancesAsync(Expression<Func<CultureAssistance, bool>>? filter = null, string? includeProperties = null)
        {
            var response = await _CultureAssistanceRepository.GetAll(filter, includeProperties);
            return response;
        }

        public async Task<RepositoryResponse<bool>> UpdateCultureAssistance(CultureAssistance CultureAssistance)
        {
            var response = await _CultureAssistanceRepository.Update(CultureAssistance);
            if(response.Success==false)
            {
                return response;
            }
            response = await _CultureAssistanceRepository.SaveChangesAsync();
            return response;
        }
    }
}
