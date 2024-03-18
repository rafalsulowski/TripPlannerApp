using Microsoft.EntityFrameworkCore;
using TripPlanner.DataAccess.IRepository;
using TripPlanner.DataAccess.Repository;
using TripPlanner.Models;
using TripPlanner.Models.Models;
using TripPlanner.Models.Models.MessageModels.QuestionnaireModels;

namespace TripPlanner.DataAccess.Repository
{
    public class QuestionnaireAnswerRepository : Repository<QuestionnaireAnswer>, IQuestionnaireAnswerRepository
    {
        private ApplicationDbContext _context;
        public QuestionnaireAnswerRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<RepositoryResponse<bool>> Update(QuestionnaireAnswer post)
        {
            _context.Entry(post).State = EntityState.Detached;
            _context.Entry(post).State = EntityState.Modified;
            return new RepositoryResponse<bool> { Data = true };
        }
    }
}
