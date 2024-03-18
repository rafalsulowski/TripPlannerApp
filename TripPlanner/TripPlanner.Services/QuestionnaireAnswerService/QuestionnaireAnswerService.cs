using System.Linq.Expressions;
using TripPlanner.DataAccess.IRepository;
using TripPlanner.Models.Models;
using TripPlanner.Models.Models.MessageModels.QuestionnaireModels;

namespace TripPlanner.Services.QuestionnaireAnswerService
{
    public class QuestionnaireAnswerService : IQuestionnaireAnswerService
    {
        private readonly IQuestionnaireAnswerRepository _QuestionnaireAnswerRepository;
        public QuestionnaireAnswerService(IQuestionnaireAnswerRepository QuestionnaireAnswerRepository)
        {
            _QuestionnaireAnswerRepository = QuestionnaireAnswerRepository;
        }

        public async Task<RepositoryResponse<bool>> CreateQuestionnaireAnswer(QuestionnaireAnswer QuestionnaireAnswer)
        {
            _QuestionnaireAnswerRepository.Add(QuestionnaireAnswer);
            var response = await _QuestionnaireAnswerRepository.SaveChangesAsync();
            return response;
        }

        public async Task<RepositoryResponse<bool>> DeleteQuestionnaireAnswer(QuestionnaireAnswer QuestionnaireAnswer)
        {
            _QuestionnaireAnswerRepository.Remove(QuestionnaireAnswer);
            var response = await _QuestionnaireAnswerRepository.SaveChangesAsync();
            return response;
        }

        public async Task<RepositoryResponse<QuestionnaireAnswer>> GetQuestionnaireAnswerAsync(Expression<Func<QuestionnaireAnswer, bool>> filter, string? includeProperties = null)
        {
            var response = await _QuestionnaireAnswerRepository.GetFirstOrDefault(filter, includeProperties);
            return response;
        }

        public async Task<RepositoryResponse<List<QuestionnaireAnswer>>> GetQuestionnaireAnswersAsync(Expression<Func<QuestionnaireAnswer, bool>>? filter = null, string? includeProperties = null)
        {
            var response = await _QuestionnaireAnswerRepository.GetAll(filter, includeProperties);
            return response;
        }

        public async Task<RepositoryResponse<bool>> UpdateQuestionnaireAnswer(QuestionnaireAnswer QuestionnaireAnswer)
        {
            var response = await _QuestionnaireAnswerRepository.Update(QuestionnaireAnswer);
            if(response.Success==false)
            {
                return response;
            }
            response = await _QuestionnaireAnswerRepository.SaveChangesAsync();
            return response;
        }
    }
}
