using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using TripPlanner.Models;
using TripPlanner.Models.DTO.MessageDTOs.QuestionnaireDTOs;
using TripPlanner.Models.Models;
using TripPlanner.Models.Models.MessageModels.QuestionnaireModels;

namespace TripPlanner.Services.QuestionnaireService
{
    public interface IQuestionnaireService
    {
        Task<RepositoryResponse<List<Questionnaire>>> GetQuestionnairesAsync(Expression<Func<Questionnaire, bool>>? filter = null, string? includeProperties = null);
        Task<RepositoryResponse<Questionnaire>> GetQuestionnaireAsync(Expression<Func<Questionnaire, bool>> filter, string? includeProperties = null);
        Task<RepositoryResponse<List<QuestionnaireAnswer>>> GetAnswersAsync(Expression<Func<QuestionnaireAnswer, bool>>? filter = null, string? includeProperties = null);
        Task<RepositoryResponse<QuestionnaireAnswer>> GetAnswerAsync(Expression<Func<QuestionnaireAnswer, bool>> filter, string? includeProperties = null);
        Task<RepositoryResponse<List<QuestionnaireVote>>> GetVotesAsync(Expression<Func<QuestionnaireVote, bool>>? filter = null, string? includeProperties = null);
        Task<RepositoryResponse<QuestionnaireVote>> GetVoteAsync(Expression<Func<QuestionnaireVote, bool>> filter, string? includeProperties = null);
        Task<RepositoryResponse<bool>> CreateQuestionnaire(Questionnaire Questionnaire);
        Task<RepositoryResponse<bool>> UpdateQuestionnaire(Questionnaire Questionnaire);
        Task<RepositoryResponse<bool>> DeleteQuestionnaire(Questionnaire Questionnaire);
        Task<RepositoryResponse<bool>> AddAnswerToQuestionnaire(QuestionnaireAnswer Answer);
        Task<RepositoryResponse<bool>> UpdateAnswer(QuestionnaireAnswer Answer);
        Task<RepositoryResponse<bool>> DeleteAnswerFromQuestionnaire(QuestionnaireAnswer Answer);
        Task<RepositoryResponse<bool>> AddVoteToAnswer(CreateQuestionnaireVoteDTO Expenditure);
        Task<RepositoryResponse<bool>> DeleteVoteFromAnswer(QuestionnaireVote Expenditure);
    }
}
