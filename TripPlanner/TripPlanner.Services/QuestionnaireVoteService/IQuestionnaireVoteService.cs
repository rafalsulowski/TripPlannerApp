using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using TripPlanner.Models;
using TripPlanner.Models.Models;
using TripPlanner.Models.Models.MessageModels.QuestionnaireModels;

namespace TripPlanner.Services.QuestionnaireVoteService
{
    public interface IQuestionnaireVoteService
    {
        Task<RepositoryResponse<List<QuestionnaireVote>>> GetQuestionnaireVotesAsync(Expression<Func<QuestionnaireVote, bool>>? filter = null, string? includeProperties = null);
        Task<RepositoryResponse<QuestionnaireVote>> GetQuestionnaireVoteAsync(Expression<Func<QuestionnaireVote, bool>> filter, string? includeProperties = null);
        Task<RepositoryResponse<bool>> CreateQuestionnaireVote(QuestionnaireVote Bill);
        Task<RepositoryResponse<bool>> UpdateQuestionnaireVote(QuestionnaireVote Bill);
        Task<RepositoryResponse<bool>> DeleteQuestionnaireVote(QuestionnaireVote Bill);
    }
}
