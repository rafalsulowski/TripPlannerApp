using System.Linq.Expressions;
using TripPlanner.DataAccess.IRepository;
using TripPlanner.Models.Models;
using TripPlanner.Models.Models.CultureModels;

namespace TripPlanner.Services.CultureService
{
    public class CultureService : ICultureService
    {
        private readonly ICultureRepository _CultureRepository;
        public CultureService(ICultureRepository CultureRepository)
        {
            _CultureRepository = CultureRepository;
        }

        public async Task<RepositoryResponse<bool>> CreateCulture(Culture Culture)
        {
            _CultureRepository.Add(Culture);
            var response = await _CultureRepository.SaveChangesAsync();
            return response;
        }

        public async Task<RepositoryResponse<bool>> DeleteCulture(Culture Culture)
        {
            _CultureRepository.Remove(Culture);
            var response = await _CultureRepository.SaveChangesAsync();
            return response;
        }

        public async Task<RepositoryResponse<Culture>> GetCultureAsync(Expression<Func<Culture, bool>> filter, string? includeProperties = null)
        {
            var response = await _CultureRepository.GetFirstOrDefault(filter, includeProperties);
            return response;
        }

        public async Task<RepositoryResponse<List<Culture>>> GetCulturesAsync(Expression<Func<Culture, bool>>? filter = null, string? includeProperties = null)
        {
            var response = await _CultureRepository.GetAll(filter, includeProperties);
            return response;
        }

        public async Task<RepositoryResponse<bool>> UpdateCulture(Culture Culture)
        {
            var response = await _CultureRepository.Update(Culture);
            if(response.Success==false)
            {
                return response;
            }
            response = await _CultureRepository.SaveChangesAsync();
            return response;
        }
    }
}
