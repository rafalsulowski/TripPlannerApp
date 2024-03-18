using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using TripPlanner.Models;
using TripPlanner.Models.Models;
using TripPlanner.Models.Models.TourModels;

namespace TripPlanner.Services.ParticipantTourService
{
    public interface IParticipantTourService
    {
        Task<RepositoryResponse<List<ParticipantTour>>> GetParticipantToursAsync(Expression<Func<ParticipantTour, bool>>? filter = null, string? includeProperties = null);
        Task<RepositoryResponse<ParticipantTour>> GetParticipantTourAsync(Expression<Func<ParticipantTour, bool>> filter, string? includeProperties = null);
        Task<RepositoryResponse<bool>> CreateParticipantTour(ParticipantTour Bill);
        Task<RepositoryResponse<bool>> UpdateParticipantTour(ParticipantTour Bill);
        Task<RepositoryResponse<bool>> DeleteParticipantTour(ParticipantTour Bill);
    }
}
