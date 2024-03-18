using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TripPlanner.Models.Models.RouteModels;
using TripPlanner.Models.Models;
using TripPlanner.Models.Models.ScheduleModels;
using TripPlanner.DataAccess.IRepository;

namespace TripPlanner.DataAccess.Repository
{
    public class ScheduleEventRepository : Repository<ScheduleEvent>, IScheduleEventRepository
    {
        private ApplicationDbContext _context;
        public ScheduleEventRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<RepositoryResponse<bool>> Update(ScheduleEvent post)
        {
            var postDB = await GetFirstOrDefault(u => u.Id == post.Id);
            if (postDB == null)
            {
                return new RepositoryResponse<bool>
                {
                    Success = false,
                    Data = false,
                    Message = "Stopover with this Id was not found."
                };
            }
            _context.ScheduleEvents.Attach(post);
            _context.Entry(post).State = EntityState.Modified;
            return new RepositoryResponse<bool> { Data = true };
        }
    }
}
