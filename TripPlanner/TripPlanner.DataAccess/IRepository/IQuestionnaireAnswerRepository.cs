using TripPlanner.Models;
using TripPlanner.Models.Models;
using TripPlanner.Models.Models.MessageModels.QuestionnaireModels;

namespace TripPlanner.DataAccess.IRepository
{
    public interface IQuestionnaireAnswerRepository : IRepository<QuestionnaireAnswer>
    {
        Task<RepositoryResponse<bool>> Update(QuestionnaireAnswer post);
    }
}
