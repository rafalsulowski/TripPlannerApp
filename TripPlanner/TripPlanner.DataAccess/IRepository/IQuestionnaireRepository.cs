using TripPlanner.Models;
using TripPlanner.Models.Models;
using TripPlanner.Models.Models.MessageModels.QuestionnaireModels;

namespace TripPlanner.DataAccess.IRepository
{
    public interface IQuestionnaireRepository : IRepository<Questionnaire>
    {
        Task<RepositoryResponse<bool>> Update(Questionnaire post);
        Task<RepositoryResponse<bool>> AddAnswerToQuestionnaire(QuestionnaireAnswer Answer);
        Task<RepositoryResponse<bool>> UpdateAnswer(QuestionnaireAnswer Answer);
        Task<RepositoryResponse<bool>> DeleteAnswerFromQuestionnaire(QuestionnaireAnswer Answer);
        Task<RepositoryResponse<bool>> AddVoteToAnswer(QuestionnaireVote Expenditure);
        Task<RepositoryResponse<bool>> DeleteVoteFromAnswer(QuestionnaireVote Expenditure);
    }
}
