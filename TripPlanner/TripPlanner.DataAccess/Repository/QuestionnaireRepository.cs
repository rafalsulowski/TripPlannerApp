using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using TripPlanner.DataAccess.IRepository;
using TripPlanner.DataAccess.Repository;
using TripPlanner.Models;
using TripPlanner.Models.Models;
using TripPlanner.Models.Models.MessageModels.QuestionnaireModels;

namespace TripPlanner.DataAccess.Repository
{
    public class QuestionnaireRepository : Repository<Questionnaire>, IQuestionnaireRepository
    {
        private ApplicationDbContext _context;
        public QuestionnaireRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<RepositoryResponse<bool>> Update(Questionnaire post)
        {
            var postDB = await GetFirstOrDefault(u => u.Id == post.Id);
            if (postDB == null)
            {
                return new RepositoryResponse<bool>
                {
                    Success = false,
                    Data = false,
                    Message = "Questionnaire with this Id was not found."
                };
            }
            _context.Questionnaires.Attach(post);
            _context.Entry(post).State = EntityState.Modified;
            return new RepositoryResponse<bool> { Data = true };
        }

        public async Task<RepositoryResponse<bool>> AddAnswerToQuestionnaire(QuestionnaireAnswer Answer)
        {
            var AnswerDB = _context.QuestionnaireAnswers.FirstOrDefault(u => u.QuestionnaireId == Answer.QuestionnaireId && u.Id == Answer.Id);
            if (AnswerDB == null)
            {
                _context.QuestionnaireAnswers.Add(Answer);
            }
            else
            {
                _context.QuestionnaireAnswers.Attach(Answer);
                _context.Entry(Answer).State = EntityState.Modified;
            }
            return new RepositoryResponse<bool> { Data = true };
        }

        public async Task<RepositoryResponse<bool>> UpdateAnswer(QuestionnaireAnswer Answer)
        {
            var AnswerDB = _context.QuestionnaireAnswers.FirstOrDefault(u => u.QuestionnaireId == Answer.QuestionnaireId);
            if (AnswerDB == null)
            {
                return new RepositoryResponse<bool>
                {
                    Success = false,
                    Data = false,
                    Message = "Nie istnieje taki członek budzetu"
                };
            }
            _context.Entry(AnswerDB).State = EntityState.Detached;
            _context.QuestionnaireAnswers.Attach(Answer);
            _context.Entry(Answer).State = EntityState.Modified;
            return new RepositoryResponse<bool> { Data = true };
        }

        public async Task<RepositoryResponse<bool>> DeleteAnswerFromQuestionnaire(QuestionnaireAnswer Answer)
        {
            var res = _context.QuestionnaireAnswers.FirstOrDefault(u => u.QuestionnaireId == Answer.QuestionnaireId);
            if (res != null)
            {
                _context.QuestionnaireAnswers.Remove(res);
            }
            return new RepositoryResponse<bool> { Data = true };
        }

        public async Task<RepositoryResponse<bool>> AddVoteToAnswer(QuestionnaireVote Vote)
        {
            //jesli jest juz taki glos to go pobierz, jesli nie to null
            var VoteDB = _context.QuestionnaireVotes.AsNoTracking().FirstOrDefault(u => u.QuestionnaireAnswerId == Vote.QuestionnaireAnswerId && u.UserId == Vote.UserId);

            //sprawdzenie czy uzytkownik nie zaglosowal na inna odpowiedz w tej ankiecie
            var answerDB = _context.QuestionnaireAnswers.AsNoTracking().FirstOrDefault(u => u.Id == Vote.QuestionnaireAnswerId);
            if (answerDB is null)
            {
                return new RepositoryResponse<bool> { Data = false, Message = $"Nie udało się odnaleźć odpowiedzi o id = {Vote.QuestionnaireAnswerId}" };
            }

            var otherAnswers = await _context.QuestionnaireAnswers.Where(u => u.QuestionnaireId == answerDB.QuestionnaireId).Include("Votes").AsNoTracking().ToListAsync();

            foreach (var answser in otherAnswers)
            {
                var similarVote = answser.Votes.FirstOrDefault(u => u.UserId == Vote.UserId);
                if (similarVote != null)
                {
                    _context.QuestionnaireVotes.Remove(similarVote);
                    _context.SaveChanges();
                }
            }

            _context.QuestionnaireVotes.Add(Vote);

            //copy answers to local list
            List<QuestionnaireAnswer> ansCopy = new List<QuestionnaireAnswer>(otherAnswers);
            ansCopy.FirstOrDefault(u => u.Id == Vote.QuestionnaireAnswerId).Votes.Add(Vote);

            int sum = 0;
            foreach (var ans in ansCopy)
                sum += ans.Votes.Count;
            foreach (var answser in ansCopy)
            {
                if (sum == 0)
                    answser.Share = 0;
                else
                    answser.Share = (double)answser.Votes.Count / sum;
                _context.QuestionnaireAnswers.Update(answser);
                _context.SaveChanges();
            }

            var questionnaire = await _context.Questionnaires.FirstOrDefaultAsync(u => u.Id == answerDB.QuestionnaireId);
            if (questionnaire is null)
                return new RepositoryResponse<bool> { Data = false, Message = $"Nie udało się odnaleźć ankiety o id = {answerDB.QuestionnaireId}" };
            
            questionnaire.VoteForLabel = $"Zagłosowano na {answerDB.Answer}";
            _context.Questionnaires.Update(questionnaire);
            _context.SaveChanges();

            return new RepositoryResponse<bool> { Data = true };
        }

        public async Task<RepositoryResponse<bool>> DeleteVoteFromAnswer(QuestionnaireVote Vote)
        {
            var res = _context.QuestionnaireVotes.FirstOrDefault(u => u.UserId == Vote.UserId && u.QuestionnaireAnswerId == Vote.QuestionnaireAnswerId);
            if (res != null)
            {
                _context.QuestionnaireVotes.Remove(res);
            }
            return new RepositoryResponse<bool> { Data = true };
        }
    }
}
