using System.Linq.Expressions;
using TripPlanner.Models.Models;
using TripPlanner.Models.Models.UserModels;

namespace TripPlanner.Services.ActiviceCodeService
{
    public interface IActiviteCodeService
    {
        Task<RepositoryResponse<ActiviteCode>> GetActiviteCodeOfUser(string code);
        Task<RepositoryResponse<bool>> CreateActiviteCode(ActiviteCode code);
        Task<RepositoryResponse<bool>> DeleteActiviteCode(ActiviteCode code);
    }
}
