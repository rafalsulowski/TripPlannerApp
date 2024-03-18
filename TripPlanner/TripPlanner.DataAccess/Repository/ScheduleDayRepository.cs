using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TripPlanner.DataAccess.IRepository;
using TripPlanner.Models.Models;
using TripPlanner.Models.Models.RouteModels;
using TripPlanner.Models.Models.ScheduleModels;

namespace TripPlanner.DataAccess.Repository
{
    public class ScheduleDayRepository : Repository<ScheduleDay>, IScheduleDayRepository
    {
        private ApplicationDbContext _context;
        public ScheduleDayRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<RepositoryResponse<bool>> AddScheduleEvent(ScheduleEvent ScheduleEvent)
        {
            _context.ScheduleEvents.Add(ScheduleEvent);
            return new RepositoryResponse<bool> { Success = true, Data = true };
        }

        public async Task<RepositoryResponse<bool>> UpdateScheduleEvent(ScheduleEvent ScheduleEvent)
        {
            var ScheduleEventDB = _context.ScheduleEvents.FirstOrDefault(u => u.ScheduleDayId == ScheduleEvent.ScheduleDayId && u.Id == ScheduleEvent.Id);
            if (ScheduleEventDB == null)
            {
                return new RepositoryResponse<bool>
                {
                    Success = false,
                    Data = false,
                    Message = "Nie istnieje take wydarzenie w harmonogramie"
                };
            }
            _context.Entry(ScheduleEventDB).State = EntityState.Detached;
            _context.ScheduleEvents.Attach(ScheduleEvent);
            _context.Entry(ScheduleEvent).State = EntityState.Modified;
            return new RepositoryResponse<bool> { Data = true };
        }

        public async Task<RepositoryResponse<bool>> DeleteScheduleEvent(ScheduleEvent ScheduleEvent)
        {
            var res = _context.ScheduleEvents.FirstOrDefault(u => u.Id == ScheduleEvent.Id && u.ScheduleDayId == ScheduleEvent.ScheduleDayId);
            if (res != null)
            {
                _context.ScheduleEvents.Remove(res);
            }
            return new RepositoryResponse<bool> { Data = true };
        }
    }
}
