using Microsoft.EntityFrameworkCore;
using TripPlanner.DataAccess.IRepository;
using TripPlanner.DataAccess.Repository;
using TripPlanner.Models;
using TripPlanner.Models.Models;
using TripPlanner.Models.Models.MessageModels.QuestionnaireModels;

namespace TripPlanner.DataAccess.Repository
{
    public class QuestionnaireVoteRepository : Repository<QuestionnaireVote>, IQuestionnaireVoteRepository
    {
        private ApplicationDbContext _context;
        public QuestionnaireVoteRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<RepositoryResponse<bool>> Update(QuestionnaireVote post)
        {
            var postDB = await GetFirstOrDefault(u => u.QuestionnaireAnswerId == post.QuestionnaireAnswerId && u.UserId == post.UserId);
            if (postDB == null)
            {
                return new RepositoryResponse<bool>
                {
                    Success = false,
                    Data = false,
                    Message = "QuestionnaireVote with this Id was not found."
                };
            }
            _context.QuestionnaireVotes.Attach(post);
            _context.Entry(post).State = EntityState.Modified;
            return new RepositoryResponse<bool> { Data = true };
        }
    }
}
