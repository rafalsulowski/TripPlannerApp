using TripPlanner.Models;
using TripPlanner.Models.Models;
using TripPlanner.Models.Models.CultureModels;
using TripPlanner.Models.Models.TourModels;

namespace TripPlanner.DataAccess.IRepository
{
    public interface ITourRepository : IRepository<Tour>
    {
        Task<RepositoryResponse<bool>> Update(Tour post);
        Task<RepositoryResponse<bool>> AddParticipantToTour(ParticipantTour Contribute);
        Task<RepositoryResponse<bool>> DeleteParticipantFromTour(ParticipantTour Contribute);
        Task<RepositoryResponse<bool>> AddCultureAssistanceToTour(CultureAssistance Contribute);
        Task<RepositoryResponse<bool>> DeleteCultureAssistanceFromTour(CultureAssistance Contribute);
    }
}
