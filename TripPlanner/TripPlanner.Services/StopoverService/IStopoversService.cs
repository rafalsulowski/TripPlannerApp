using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using TripPlanner.Models;
using TripPlanner.Models.Models;
using TripPlanner.Models.Models.RouteModels;

namespace TripPlanner.Services.StopoverService
{
    public interface IStopoverService
    {
        Task<RepositoryResponse<List<Stopover>>> GetStopoversAsync(Expression<Func<Stopover, bool>>? filter = null, string? includeProperties = null);
        Task<RepositoryResponse<Stopover>> GetStopoverAsync(Expression<Func<Stopover, bool>> filter, string? includeProperties = null);
        Task<RepositoryResponse<bool>> CreateStopover(Stopover Bill);
        Task<RepositoryResponse<bool>> UpdateStopover(Stopover Bill);
        Task<RepositoryResponse<bool>> DeleteStopover(Stopover Bill);
    }
}
