using TripPlanner.Models.Models;
using TripPlanner.Models.Models.ScheduleModels;
using TripPlanner.DataAccess.IRepository;

namespace TripPlanner.Services.ScheduleService
{
    public class ScheduleService : IScheduleService
    {
        private readonly IScheduleDayRepository _ScheduleDayRepository;
        private readonly IScheduleEventRepository _ScheduleEventRepository;
        public ScheduleService(IScheduleDayRepository ScheduleRepository, IScheduleEventRepository ScheduleEventRepository)
        {
            _ScheduleDayRepository = ScheduleRepository;
            _ScheduleEventRepository = ScheduleEventRepository;
        }

        public async Task<RepositoryResponse<bool>> CreateScheduleDay(ScheduleDay ScheduleDay)
        {
            _ScheduleDayRepository.Add(ScheduleDay);
            var response = await _ScheduleDayRepository.SaveChangesAsync();
            return response;
        }

        public async Task<RepositoryResponse<bool>> DeleteScheduleDay(ScheduleDay ScheduleDay)
        {
            _ScheduleDayRepository.Remove(ScheduleDay);
            var response = await _ScheduleDayRepository.SaveChangesAsync();
            return response;
        }

        public async Task<RepositoryResponse<List<ScheduleDay>>> GetWholeSchedule(int tourId)
        {
            var response = await _ScheduleDayRepository.GetAll(p => p.TourId == tourId, "Events");
            return response;
        }

        public async Task<RepositoryResponse<ScheduleDay>> GetScheduleDay(int scheduleDayId)
        {
            var response = await _ScheduleDayRepository.GetFirstOrDefault(u => u.Id == scheduleDayId, "Events");
            return response;
        }

        public async Task<RepositoryResponse<bool>> CreateScheduleEvent(ScheduleEvent Event)
        {
            await _ScheduleDayRepository.AddScheduleEvent(Event);
            return await _ScheduleDayRepository.SaveChangesAsync();
        }

        public async Task<RepositoryResponse<bool>> UpdateScheduleEvent(ScheduleEvent Event)
        {
            var response = await _ScheduleDayRepository.UpdateScheduleEvent(Event);
            if (response.Success == false)
            {
                return response;
            }
            response = await _ScheduleDayRepository.SaveChangesAsync();
            return response;
        }

        public async Task<RepositoryResponse<bool>> DeleteScheduleEvent(ScheduleEvent Event)
        {
            await _ScheduleDayRepository.DeleteScheduleEvent(Event);
            return await _ScheduleDayRepository.SaveChangesAsync();
        }
    }
}
