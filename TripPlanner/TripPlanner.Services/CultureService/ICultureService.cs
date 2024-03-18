using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using TripPlanner.Models;
using TripPlanner.Models.Models;
using TripPlanner.Models.Models.CultureModels;

namespace TripPlanner.Services.CultureService
{
    public interface ICultureService
    {
        Task<RepositoryResponse<List<Culture>>> GetCulturesAsync(Expression<Func<Culture, bool>>? filter = null, string? includeProperties = null);
        Task<RepositoryResponse<Culture>> GetCultureAsync(Expression<Func<Culture, bool>> filter, string? includeProperties = null);
        Task<RepositoryResponse<bool>> CreateCulture(Culture Bill);
        Task<RepositoryResponse<bool>> UpdateCulture(Culture Bill);
        Task<RepositoryResponse<bool>> DeleteCulture(Culture Bill);
    }
}
