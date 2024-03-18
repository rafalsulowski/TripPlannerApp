using System.Linq.Expressions;
using TripPlanner.DataAccess.IRepository;
using TripPlanner.Models.Models;
using TripPlanner.Models.Models.MessageModels.QuestionnaireModels;

namespace TripPlanner.Services.QuestionnaireVoteService
{
    public class QuestionnaireVoteService : IQuestionnaireVoteService
    {
        private readonly IQuestionnaireVoteRepository _QuestionnaireVoteRepository;
        public QuestionnaireVoteService(IQuestionnaireVoteRepository QuestionnaireVoteRepository)
        {
            _QuestionnaireVoteRepository = QuestionnaireVoteRepository;
        }

        public async Task<RepositoryResponse<bool>> CreateQuestionnaireVote(QuestionnaireVote QuestionnaireVote)
        {
            _QuestionnaireVoteRepository.Add(QuestionnaireVote);
            var response = await _QuestionnaireVoteRepository.SaveChangesAsync();
            return response;
        }

        public async Task<RepositoryResponse<bool>> DeleteQuestionnaireVote(QuestionnaireVote QuestionnaireVote)
        {
            _QuestionnaireVoteRepository.Remove(QuestionnaireVote);
            var response = await _QuestionnaireVoteRepository.SaveChangesAsync();
            return response;
        }

        public async Task<RepositoryResponse<QuestionnaireVote>> GetQuestionnaireVoteAsync(Expression<Func<QuestionnaireVote, bool>> filter, string? includeProperties = null)
        {
            var response = await _QuestionnaireVoteRepository.GetFirstOrDefault(filter, includeProperties);
            return response;
        }

        public async Task<RepositoryResponse<List<QuestionnaireVote>>> GetQuestionnaireVotesAsync(Expression<Func<QuestionnaireVote, bool>>? filter = null, string? includeProperties = null)
        {
            var response = await _QuestionnaireVoteRepository.GetAll(filter, includeProperties);
            return response;
        }

        public async Task<RepositoryResponse<bool>> UpdateQuestionnaireVote(QuestionnaireVote QuestionnaireVote)
        {
            var response = await _QuestionnaireVoteRepository.Update(QuestionnaireVote);
            if(response.Success==false)
            {
                return response;
            }
            response = await _QuestionnaireVoteRepository.SaveChangesAsync();
            return response;
        }
    }
}
