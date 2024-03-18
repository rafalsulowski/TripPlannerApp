using System.Linq.Expressions;
using TripPlanner.DataAccess.IRepository;
using TripPlanner.Models;
using TripPlanner.Models.Models;
using TripPlanner.Models.Models.RouteModels;

namespace TripPlanner.Services.StopoverService
{
    public class StopoverService : IStopoverService
    {
        private readonly IStopoverRepository _StopoverRepository;
        public StopoverService(IStopoverRepository StopoverRepository)
        {
            _StopoverRepository = StopoverRepository;
        }

        public async Task<RepositoryResponse<bool>> CreateStopover(Stopover Stopover)
        {
            _StopoverRepository.Add(Stopover);
            var response = await _StopoverRepository.SaveChangesAsync();
            return response;
        }

        public async Task<RepositoryResponse<bool>> DeleteStopover(Stopover Stopover)
        {
            _StopoverRepository.Remove(Stopover);
            var response = await _StopoverRepository.SaveChangesAsync();
            return response;
        }

        public async Task<RepositoryResponse<Stopover>> GetStopoverAsync(Expression<Func<Stopover, bool>> filter, string? includeProperties = null)
        {
            var response = await _StopoverRepository.GetFirstOrDefault(filter, includeProperties);
            return response;
        }

        public async Task<RepositoryResponse<List<Stopover>>> GetStopoversAsync(Expression<Func<Stopover, bool>>? filter = null, string? includeProperties = null)
        {
            var response = await _StopoverRepository.GetAll(filter, includeProperties);
            return response;
        }

        public async Task<RepositoryResponse<bool>> UpdateStopover(Stopover Stopover)
        {
            var response = await _StopoverRepository.Update(Stopover);
            if(response.Success==false)
            {
                return response;
            }
            response = await _StopoverRepository.SaveChangesAsync();
            return response;
        }
    }
}
