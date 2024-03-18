using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using TripPlanner.Models;
using TripPlanner.Models.Models;
using TripPlanner.Models.Models.MessageModels.QuestionnaireModels;

namespace TripPlanner.Services.QuestionnaireAnswerService
{
    public interface IQuestionnaireAnswerService
    {
        Task<RepositoryResponse<List<QuestionnaireAnswer>>> GetQuestionnaireAnswersAsync(Expression<Func<QuestionnaireAnswer, bool>>? filter = null, string? includeProperties = null);
        Task<RepositoryResponse<QuestionnaireAnswer>> GetQuestionnaireAnswerAsync(Expression<Func<QuestionnaireAnswer, bool>> filter, string? includeProperties = null);
        Task<RepositoryResponse<bool>> CreateQuestionnaireAnswer(QuestionnaireAnswer Bill);
        Task<RepositoryResponse<bool>> UpdateQuestionnaireAnswer(QuestionnaireAnswer Bill);
        Task<RepositoryResponse<bool>> DeleteQuestionnaireAnswer(QuestionnaireAnswer Bill);
    }
}
