using TripPlanner.Models;
using TripPlanner.Models.Models;
using TripPlanner.Models.Models.TourModels;

namespace TripPlanner.DataAccess.IRepository
{
    public interface IParticipantTourRepository : IRepository<ParticipantTour>
    {
        Task<RepositoryResponse<bool>> Update(ParticipantTour post);
    }
}
