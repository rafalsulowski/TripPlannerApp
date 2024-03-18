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
    public class NoticeMessageRepository : Repository<NoticeMessage>, INoticeMessageRepository
    {
        private ApplicationDbContext _context;
        public NoticeMessageRepository(ApplicationDbContext context) : base(context)
        {
            
            _context = context;
        }

        public async Task<RepositoryResponse<bool>> Update(NoticeMessage post)
        {
            var postDB = await GetFirstOrDefault(u => u.Id == post.Id);
            if (postDB == null)
            {
                return new RepositoryResponse<bool>
                {
                    Success = false,
                    Data = false,
                    Message = "Notice Message with this Id was not found."
                };
            }

            _context.NoticeMessages.Attach(post);
            _context.Entry(post).State = EntityState.Modified;
            return new RepositoryResponse<bool> { Data = true };
        }
    }
}
