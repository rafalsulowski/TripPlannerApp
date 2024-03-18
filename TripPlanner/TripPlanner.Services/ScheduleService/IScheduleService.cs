using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using TripPlanner.Models.Models.RouteModels;
using TripPlanner.Models.Models;
using TripPlanner.Models.Models.ScheduleModels;

namespace TripPlanner.Services.ScheduleService
{
    public interface IScheduleService
    {
        Task<RepositoryResponse<bool>> CreateScheduleDay(ScheduleDay schedule);
        Task<RepositoryResponse<bool>> DeleteScheduleDay(ScheduleDay schedule);
        Task<RepositoryResponse<List<ScheduleDay>>> GetWholeSchedule(int tourId);
        Task<RepositoryResponse<ScheduleDay>> GetScheduleDay(int scheduleDayId);
        Task<RepositoryResponse<bool>> CreateScheduleEvent(ScheduleEvent newEvent);
        Task<RepositoryResponse<bool>> UpdateScheduleEvent(ScheduleEvent newEvent);
        Task<RepositoryResponse<bool>> DeleteScheduleEvent(ScheduleEvent e);
    }
}
