using System.Linq.Expressions;
using TripPlanner.Models.DTO.TourDTOs;
using TripPlanner.Models.Models;
using TripPlanner.Models.Models.CultureModels;
using TripPlanner.Models.Models.TourModels;

namespace TripPlanner.Services.TourService
{
    public interface ITourService
    {
        Task<RepositoryResponse<List<Tour>>> GetToursAsync(Expression<Func<Tour, bool>>? filter = null, string? includeProperties = null);
        Task<RepositoryResponse<Tour>> GetTourAsync(Expression<Func<Tour, bool>> filter, string? includeProperties = null);
        Task<RepositoryResponse<List<ParticipantTour>>> GetParticipantsAsync(Expression<Func<ParticipantTour, bool>>? filter = null, string? includeProperties = null);
        Task<RepositoryResponse<List<ExtendParticipantDTO>>> GetTourExtendParticipants(int tourId);
        Task<RepositoryResponse<ExtendParticipantDTO>> GetTourExtendParticipantById(int tourId, int userId);
        Task<RepositoryResponse<List<ExtendParticipantDTO>>> GetParticipantsNames(int tourId);
        Task<RepositoryResponse<ParticipantTour>> GetParticipantAsync(Expression<Func<ParticipantTour, bool>> filter, string? includeProperties = null);
        Task<RepositoryResponse<List<CultureAssistance>>> GetCulturesAssistanceAsync(Expression<Func<CultureAssistance, bool>>? filter = null, string? includeProperties = null);
        Task<RepositoryResponse<CultureAssistance>> GetCultureAssistanceAsync(Expression<Func<CultureAssistance, bool>> filter, string? includeProperties = null);
        Task<RepositoryResponse<bool>> CreateTour(Tour Bill, int userId);
        Task<RepositoryResponse<bool>> UpdateTour(Tour Bill);
        Task<RepositoryResponse<bool>> DeleteTour(Tour Bill);
        Task<RepositoryResponse<bool>> AddParticipantToTour(ParticipantTour Contribute);
        Task<RepositoryResponse<bool>> DeleteParticipantFromTour(ParticipantTour Contribute);
        Task<RepositoryResponse<bool>> AddCultureAssistanceToTour(CultureAssistance Contribute);
        Task<RepositoryResponse<bool>> DeleteCultureAssistanceFromTour(CultureAssistance Contribute);
        int CalculateAge(DateTime dt1, DateTime dt2);
    }
}
