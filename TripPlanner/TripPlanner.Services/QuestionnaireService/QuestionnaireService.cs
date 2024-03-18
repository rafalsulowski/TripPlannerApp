using System.Linq.Expressions;
using TripPlanner.DataAccess.IRepository;
using TripPlanner.Models.DTO.MessageDTOs.QuestionnaireDTOs;
using TripPlanner.Models.Models;
using TripPlanner.Models.Models.MessageModels.QuestionnaireModels;
using TripPlanner.Services.QuestionnaireAnswerService;

namespace TripPlanner.Services.QuestionnaireService
{
    public class QuestionnaireService : IQuestionnaireService
    {
        private readonly IQuestionnaireRepository _QuestionnaireRepository;
        private readonly IQuestionnaireAnswerService _QuestionnaireAnswerService;
        private readonly IQuestionnaireAnswerRepository _QuestionnaireAnswerRepository;
        private readonly IQuestionnaireVoteRepository _QuestionnaireVoteRepository;
        public QuestionnaireService(IQuestionnaireRepository QuestionnaireRepository, IQuestionnaireAnswerService questionnaireAnswerService, IQuestionnaireAnswerRepository QuestionnaireAnswerRepository, IQuestionnaireVoteRepository QuestionnaireVoteRepository)
        {
            _QuestionnaireRepository = QuestionnaireRepository;
            _QuestionnaireAnswerRepository = QuestionnaireAnswerRepository;
            _QuestionnaireVoteRepository = QuestionnaireVoteRepository;
            _QuestionnaireAnswerService = questionnaireAnswerService;
        }

        public async Task<RepositoryResponse<bool>> CreateQuestionnaire(Questionnaire Questionnaire)
        {
            _QuestionnaireRepository.Add(Questionnaire);
            var response = await _QuestionnaireRepository.SaveChangesAsync();
            return response;
        }

        public async Task<RepositoryResponse<bool>> DeleteQuestionnaire(Questionnaire Questionnaire)
        {
            _QuestionnaireRepository.Remove(Questionnaire);
            var response = await _QuestionnaireRepository.SaveChangesAsync();
            return response;
        }

        public async Task<RepositoryResponse<Questionnaire>> GetQuestionnaireAsync(Expression<Func<Questionnaire, bool>> filter, string? includeProperties = null)
        {
            var response = await _QuestionnaireRepository.GetFirstOrDefault(filter, includeProperties);
            return response;
        }

        public async Task<RepositoryResponse<List<Questionnaire>>> GetQuestionnairesAsync(Expression<Func<Questionnaire, bool>>? filter = null, string? includeProperties = null)
        {
            var response = await _QuestionnaireRepository.GetAll(filter, includeProperties);
            return response;
        }

        public async Task<RepositoryResponse<QuestionnaireAnswer>> GetAnswerAsync(Expression<Func<QuestionnaireAnswer, bool>> filter, string? includeProperties = null)
        {
            var response = await _QuestionnaireAnswerRepository.GetFirstOrDefault(filter, includeProperties);
            return response;
        }

        public async Task<RepositoryResponse<List<QuestionnaireAnswer>>> GetAnswersAsync(Expression<Func<QuestionnaireAnswer, bool>>? filter = null, string? includeProperties = null)
        {
            var response = await _QuestionnaireAnswerRepository.GetAll(filter, includeProperties);
            return response;
        }

        public async Task<RepositoryResponse<QuestionnaireVote>> GetVoteAsync(Expression<Func<QuestionnaireVote, bool>> filter, string? includeProperties = null)
        {
            var response = await _QuestionnaireVoteRepository.GetFirstOrDefault(filter, includeProperties);
            return response;
        }

        public async Task<RepositoryResponse<List<QuestionnaireVote>>> GetVotesAsync(Expression<Func<QuestionnaireVote, bool>>? filter = null, string? includeProperties = null)
        {
            var response = await _QuestionnaireVoteRepository.GetAll(filter, includeProperties);
            return response;
        }

        public async Task<RepositoryResponse<bool>> UpdateQuestionnaire(Questionnaire Questionnaire)
        {
            var response = await _QuestionnaireRepository.Update(Questionnaire);
            if (response.Success == false)
            {
                return response;
            }
            response = await _QuestionnaireRepository.SaveChangesAsync();
            return response;
        }

        public async Task<RepositoryResponse<bool>> AddAnswerToQuestionnaire(QuestionnaireAnswer Answer)
        {
            await _QuestionnaireRepository.AddAnswerToQuestionnaire(Answer);
            return await _QuestionnaireRepository.SaveChangesAsync();
        }

        public async Task<RepositoryResponse<bool>> UpdateAnswer(QuestionnaireAnswer Answer)
        {
            var response = await _QuestionnaireRepository.UpdateAnswer(Answer);
            if (response.Success == false)
            {
                return response;
            }
            response = await _QuestionnaireRepository.SaveChangesAsync();
            return response;
        }

        public async Task<RepositoryResponse<bool>> DeleteAnswerFromQuestionnaire(QuestionnaireAnswer Answer)
        {
            await _QuestionnaireRepository.DeleteAnswerFromQuestionnaire(Answer);
            return await _QuestionnaireRepository.SaveChangesAsync();
        }

        public async Task<RepositoryResponse<bool>> AddVoteToAnswer(CreateQuestionnaireVoteDTO Contribute)
        {
            QuestionnaireVoteDTO vote = new QuestionnaireVoteDTO();
            vote.QuestionnaireAnswerId = Contribute.AnswerId;
            vote.UserId = Contribute.UserId;
            
            await _QuestionnaireRepository.AddVoteToAnswer(vote);
            return await _QuestionnaireRepository.SaveChangesAsync();
        }

        public async Task<RepositoryResponse<bool>> DeleteVoteFromAnswer(QuestionnaireVote Contribute)
        {
            await _QuestionnaireRepository.DeleteVoteFromAnswer(Contribute);
            return await _QuestionnaireRepository.SaveChangesAsync();
        }
    }
}
