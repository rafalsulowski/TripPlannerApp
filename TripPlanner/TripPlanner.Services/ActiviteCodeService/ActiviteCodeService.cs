using System.Linq.Expressions;
using TripPlanner.DataAccess.IRepository;
using TripPlanner.DataAccess.Repository;
using TripPlanner.Models.Models;
using TripPlanner.Models.Models.UserModels;

namespace TripPlanner.Services.ActiviceCodeService
{
    public class ActiviteCodeService : IActiviteCodeService
    {
        private readonly IActiviteCodeRepository _ActiviteCodeRepository;
        public ActiviteCodeService(IActiviteCodeRepository activiteCodeService)
        {
            _ActiviteCodeRepository = activiteCodeService;
        }

        public async Task<RepositoryResponse<ActiviteCode>> GetActiviteCodeOfUser(string code)
        {
            var response = await _ActiviteCodeRepository.GetFirstOrDefault(u=> u.Code == code);
            return response;
        }

        public async Task<RepositoryResponse<bool>> CreateActiviteCode(ActiviteCode code)
        {
            _ActiviteCodeRepository.Add(code);
            return await _ActiviteCodeRepository.SaveChangesAsync();
        }


        public async Task<RepositoryResponse<bool>> DeleteActiviteCode(ActiviteCode code)
        {
            _ActiviteCodeRepository.Remove(code);
            return await _ActiviteCodeRepository.SaveChangesAsync();
        }
    }
}
