using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using TripPlanner.Models;
using TripPlanner.Models.Models;
using TripPlanner.Models.Models.CultureModels;

namespace TripPlanner.Services.CultureAssistanceService
{
    public interface ICultureAssistanceService
    {
        Task<RepositoryResponse<List<CultureAssistance>>> GetCultureAssistancesAsync(Expression<Func<CultureAssistance, bool>>? filter = null, string? includeProperties = null);
        Task<RepositoryResponse<CultureAssistance>> GetCultureAssistanceAsync(Expression<Func<CultureAssistance, bool>> filter, string? includeProperties = null);
        Task<RepositoryResponse<bool>> CreateCultureAssistance(CultureAssistance Bill);
        Task<RepositoryResponse<bool>> UpdateCultureAssistance(CultureAssistance Bill);
        Task<RepositoryResponse<bool>> DeleteCultureAssistance(CultureAssistance Bill);
    }
}
