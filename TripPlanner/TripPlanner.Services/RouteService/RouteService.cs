using System.Linq.Expressions;
using TripPlanner.DataAccess.IRepository;
using TripPlanner.Models.Models;
using TripPlanner.Models.Models.RouteModels;

namespace TripPlanner.Services.RouteService
{
    public class RouteService : IRouteService
    {
        private readonly IRouteRepository _RouteRepository;
        private readonly IStopoverRepository _StopoverRepository;
        public RouteService(IRouteRepository RouteRepository, IStopoverRepository stopoverRepository)
        {
            _RouteRepository = RouteRepository;
            _StopoverRepository = stopoverRepository;
        }

        public async Task<RepositoryResponse<bool>> CreateRoute(Route Route)
        {
            _RouteRepository.Add(Route);
            var response = await _RouteRepository.SaveChangesAsync();
            return response;
        }

        public async Task<RepositoryResponse<bool>> DeleteRoute(Route Route)
        {
            _RouteRepository.Remove(Route);
            var response = await _RouteRepository.SaveChangesAsync();
            return response;
        }

        public async Task<RepositoryResponse<Route>> GetRouteAsync(Expression<Func<Route, bool>> filter, string? includeProperties = null)
        {
            var response = await _RouteRepository.GetFirstOrDefault(filter, includeProperties);
            return response;
        }

        public async Task<RepositoryResponse<List<Route>>> GetRoutesAsync(Expression<Func<Route, bool>>? filter = null, string? includeProperties = null)
        {
            var response = await _RouteRepository.GetAll(filter, includeProperties);
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

        public async Task<RepositoryResponse<bool>> UpdateRoute(Route Route)
        {
            var response = await _RouteRepository.Update(Route);
            if(response.Success==false)
            {
                return response;
            }
            response = await _RouteRepository.SaveChangesAsync();
            return response;
        }

        public async Task<RepositoryResponse<bool>> AddStopoverToRoute(Stopover Stopover)
        {
            await _RouteRepository.AddStopoverToRoute(Stopover);
            return await _RouteRepository.SaveChangesAsync();
        }

        public async Task<RepositoryResponse<bool>> UpdateStopover(Stopover Stopover)
        {
            var response = await _RouteRepository.UpdateStopover(Stopover);
            if (response.Success == false)
            {
                return response;
            }
            response = await _RouteRepository.SaveChangesAsync();
            return response;
        }

        public async Task<RepositoryResponse<bool>> DeleteStopoverFromRoute(Stopover Stopover)
        {
            await _RouteRepository.DeleteStopoverFromRoute(Stopover);
            return await _RouteRepository.SaveChangesAsync();
        }
    }
}
