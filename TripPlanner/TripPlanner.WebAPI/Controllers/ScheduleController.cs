using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TripPlanner.Models.DTO.ScheduleDTOs;
using TripPlanner.Models.Models;
using TripPlanner.Models.Models.ScheduleModels;
using TripPlanner.Services.ScheduleService;

namespace TripPlanner.WebAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    //[ApiExplorerSettings(IgnoreApi = ProjectConfiguration.HideContorller)]
    public class ScheduleController : ControllerBase
    {
        private readonly IScheduleService _ScheduleService;

        public ScheduleController(IScheduleService ScheduleService)
        {
            _ScheduleService = ScheduleService;
        }

        [HttpGet("getSchedule/{tourId}")]
        public async Task<ActionResult<List<ScheduleDayDTO>>> GetSchedule(int tourId)
        {
            RepositoryResponse<List<ScheduleDay>> response = await _ScheduleService.GetWholeSchedule(tourId);
            return Ok(response.Data?.Select(u => (ScheduleDayDTO)u).ToList());
        }

        [HttpGet("getScheduleDay/{scheduleDayId}")]
        public async Task<ActionResult<ScheduleDayDTO>> GetScheduleDay(int scheduleDayId)
        {
            RepositoryResponse<ScheduleDay> response = await _ScheduleService.GetScheduleDay(scheduleDayId);
            return Ok((ScheduleDayDTO)response.Data);
        }


        [HttpPost("scheduleEvent")]
        public async Task<ActionResult<RepositoryResponse<bool>>> CreateScheduleEvent([FromBody] CreateScheduleEventDTO Event)
        {
            ScheduleEvent newEvent = Event;

            var response = await _ScheduleService.CreateScheduleEvent(newEvent);
            if (response.Success)
                return Ok(new RepositoryResponse<bool> { Data = true, Success = true, Message = "" });
            else
                return BadRequest(new RepositoryResponse<bool> { Data = false, Success = false, Message = response.Message });
        }

        [HttpPut("{ScheduleDayId}/editEvent/{ScheduleEventId}/{userId}")]
        public async Task<ActionResult<RepositoryResponse<bool>>> EditScheduleEvent(int ScheduleDayId, int ScheduleEventId, int userId, [FromBody] EditScheduleEventDTO editEvent)
        {
            //walidacja uzytkownika

            ScheduleEvent newEvent = editEvent;
            newEvent.ScheduleDayId = ScheduleDayId;
            newEvent.Id= ScheduleEventId;

            var response = await _ScheduleService.UpdateScheduleEvent(newEvent);
            if (response.Success)
                return Ok(new RepositoryResponse<bool> { Data = true, Success = true, Message = "" });
            else
                return BadRequest(new RepositoryResponse<bool> { Data = false, Success = false, Message = response.Message });
        }

        [HttpDelete("{ScheduleDayId}/deleteEvent/{ScheduleEventId}/{userId}")]
        public async Task<ActionResult<RepositoryResponse<bool>>> DeleteScheduleEvent(int ScheduleDayId, int ScheduleEventId, int userId)
        {
            //walidacja uzytkownika

            ScheduleEvent newEvent = new ScheduleEvent
            {
                ScheduleDayId = ScheduleDayId,
                Id = ScheduleEventId
            };
          
            var response = await _ScheduleService.DeleteScheduleEvent(newEvent);
            if (response.Success)
                return Ok(new RepositoryResponse<bool> { Data = true, Success = true, Message = "" });
            else
                return BadRequest(new RepositoryResponse<bool> { Data = false, Success = false, Message = response.Message });
        }
    }
}
