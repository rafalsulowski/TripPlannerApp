using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TripPlanner.Models.Models.MessageModels.QuestionnaireModels;
using TripPlanner.Models.Models.MessageModels;
using TripPlanner.Models.Models;
using TripPlanner.DataAccess.IRepository;

namespace TripPlanner.DataAccess.Repository
{
    public class TextMessageRepository : Repository<TextMessage>, ITextMessageRepository
    {
        private ApplicationDbContext _context;
        public TextMessageRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<RepositoryResponse<bool>> Update(TextMessage post)
        {
            var postDB = await GetFirstOrDefault(u => u.Id == post.Id);
            if (postDB == null)
            {
                return new RepositoryResponse<bool>
                {
                    Success = false,
                    Data = false,
                    Message = "Text Message with this Id was not found."
                };
            }

            _context.TextMessages.Attach(post);
            _context.Entry(post).State = EntityState.Modified;
            return new RepositoryResponse<bool> { Data = true };
        }
    }
}
