using System.Linq.Expressions;
using TripPlanner.DataAccess.IRepository;
using TripPlanner.Models.Models;
using TripPlanner.Models.Models.TourModels;

namespace TripPlanner.Services.ParticipantTourService
{
    public class ParticipantTourService : IParticipantTourService
    {
        private readonly IParticipantTourRepository _ParticipantTourRepository;
        public ParticipantTourService(IParticipantTourRepository ParticipantTourRepository)
        {
            _ParticipantTourRepository = ParticipantTourRepository;
        }

        public async Task<RepositoryResponse<bool>> CreateParticipantTour(ParticipantTour ParticipantTour)
        {
            _ParticipantTourRepository.Add(ParticipantTour);
            var response = await _ParticipantTourRepository.SaveChangesAsync();
            return response;
        }

        public async Task<RepositoryResponse<bool>> DeleteParticipantTour(ParticipantTour ParticipantTour)
        {
            _ParticipantTourRepository.Remove(ParticipantTour);
            var response = await _ParticipantTourRepository.SaveChangesAsync();
            return response;
        }

        public async Task<RepositoryResponse<ParticipantTour>> GetParticipantTourAsync(Expression<Func<ParticipantTour, bool>> filter, string? includeProperties = null)
        {
            var response = await _ParticipantTourRepository.GetFirstOrDefault(filter, includeProperties);
            return response;
        }

        public async Task<RepositoryResponse<List<ParticipantTour>>> GetParticipantToursAsync(Expression<Func<ParticipantTour, bool>>? filter = null, string? includeProperties = null)
        {
            var response = await _ParticipantTourRepository.GetAll(filter, includeProperties);
            return response;
        }

        public async Task<RepositoryResponse<bool>> UpdateParticipantTour(ParticipantTour ParticipantTour)
        {
            var response = await _ParticipantTourRepository.Update(ParticipantTour);
            if(response.Success==false)
            {
                return response;
            }
            response = await _ParticipantTourRepository.SaveChangesAsync();
            return response;
        }
    }
}
