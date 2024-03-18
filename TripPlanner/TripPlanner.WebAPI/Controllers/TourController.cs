using Microsoft.AspNetCore.Mvc;
using TripPlanner.Models.DTO.TourDTOs;
using TripPlanner.Models.DTO.CultureDTOs;
using TripPlanner.Services.CultureService;
using TripPlanner.Services.TourService;
using TripPlanner.Services.UserService;
using TripPlanner.Models.Models;
using TripPlanner.Models.Models.TourModels;
using TripPlanner.Models.Models.CultureModels;
using TripPlanner.Services.ChatService;
using TripPlanner.Services.ParticipantTourService;
using System;
using TripPlanner.Models.DTO.UserDTOs;
using TripPlanner.Models.Models.UserModels;
using Microsoft.AspNetCore.Authorization;

namespace TripPlanner.WebAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class TourController : ControllerBase
    {
        private readonly ITourService _TourService;
        private readonly IUserService _UserService;
        private readonly ICultureService _CultureService;
        private readonly IChatService _ChatService;
        private readonly IParticipantTourService _ParticipantTourService;

        public TourController(ITourService TourService, IUserService userService, ICultureService cultureService, IChatService chatService, IParticipantTourService participantTourService)
        {
            _TourService = TourService;
            _UserService = userService;
            _CultureService = cultureService;
            _ChatService = chatService;
            _ParticipantTourService = participantTourService;
        }

        // GET: api/<ValuesController>
        [HttpGet]
        public async Task<ActionResult<List<TourDTO>>> Get()
        {
            RepositoryResponse<List<Tour>> response = await _TourService.GetToursAsync();
            return Ok(response.Data?.Select(u => (TourDTO)u).ToList());
        }
        
        [HttpGet("{tourId}/getTourToPdfById")]
        public async Task<ActionResult<TourDTO>> GetTourToPdfById(int tourId)
        {
            RepositoryResponse<Tour> response = await _TourService.GetTourAsync(u => u.Id == tourId, "Participants,Schedule.Events");
            return Ok((TourDTO)response.Data);
        }

        [HttpGet("{tourId}/getWithParticipants")]
        public async Task<ActionResult<TourDTO>> GetWithParticipant(int tourId)
        {
            RepositoryResponse<Tour> response = await _TourService.GetTourAsync(u => u.Id == tourId, "Participants");
            return Ok((TourDTO)response.Data);
        }

        [HttpGet("{tourId}/getExtendParticipants")]
        public async Task<ActionResult<List<ExtendParticipantDTO>>> GetExtendParticipants(int tourId)
        {
            RepositoryResponse<List<ExtendParticipantDTO>> response = await _TourService.GetTourExtendParticipants(tourId);
            return Ok(response.Data);
        }

        [HttpGet("{tourId}/getExtendParticipantsById/{userId}")]
        public async Task<ActionResult<ExtendParticipantDTO>> GetExtendParticipantsById(int tourId, int userId)
        {
            RepositoryResponse<ExtendParticipantDTO> response = await _TourService.GetTourExtendParticipantById(tourId, userId);
            return Ok(response.Data);
        }

        [HttpGet("{tourId}/getParticipantsNames")]
        public async Task<ActionResult<List<ExtendParticipantDTO>>> GetParticipantsNames(int tourId)
        {
            RepositoryResponse<List<ExtendParticipantDTO>> response = await _TourService.GetParticipantsNames(tourId);
            return Ok(response.Data);
        }

        [HttpGet("{tourId}/getWithMessages")]
        public async Task<ActionResult<TourDTO>> GetWithMessages(int tourId)
        {
            RepositoryResponse<Tour> response = await _TourService.GetTourAsync(u => u.Id == tourId, "Messages.Answer");
            return Ok((TourDTO)response.Data);
        }

        [HttpGet("{tourId}/getWithCheckLists")]
        public async Task<ActionResult<TourDTO>> GetWithCheckLists(int tourId)
        {
            RepositoryResponse<Tour> response = await _TourService.GetTourAsync(u => u.Id == tourId, "CheckLists");
            return Ok((TourDTO)response.Data);
        }

        [HttpGet("{tourId}/getWithQuestionnaires")]
        public async Task<ActionResult<TourDTO>> GetWithQuestionnaires(int tourId)
        {
            RepositoryResponse<Tour> response = await _TourService.GetTourAsync(u => u.Id == tourId, "Messages");
            return Ok((TourDTO)response.Data);
        }


        [HttpGet("{tourId}/getWithRoutes")]
        public async Task<ActionResult<TourDTO>> GetWithRoutes(int tourId)
        {
            RepositoryResponse<Tour> response = await _TourService.GetTourAsync(u => u.Id == tourId, "Routes");
            return Ok((TourDTO)response.Data);
        }

        [HttpGet("{tourId}/getWithBills/{id}")]
        public async Task<ActionResult<TourDTO>> GetWithBills(int tourId)
        {
            RepositoryResponse<Tour> response = await _TourService.GetTourAsync(u => u.Id == tourId, "Bills");
            return Ok((TourDTO)response.Data);
        }

        [HttpGet("{tourId}/getWithCultureAssistance")]
        public async Task<ActionResult<TourDTO>> GetWithCultureAssistance(int tourId)
        {
            RepositoryResponse<Tour> response = await _TourService.GetTourAsync(u => u.Id == tourId, "Cultures");
            return Ok((TourDTO)response.Data);
        }


        // GET api/<ValuesController>/5
        [HttpGet("{tourId}")]
        public async Task<ActionResult<TourDTO>> Get(int tourId)
        {
            RepositoryResponse<Tour> response = await _TourService.GetTourAsync(u => u.Id == tourId);
            return Ok((TourDTO)response.Data);
        }

        // POST api/<ValuesController>
        [HttpPost]
        public async Task<ActionResult<RepositoryResponse<int>>> Create([FromBody] CreateTourDTO Tour)
        {
            var resp = await _UserService.GetUserAsync(u => u.Id == Tour.UserId, "ParticipantTours");
            if (resp.Data == null)
            {
                return new RepositoryResponse<int> { Data = -1, Success = false, Message = $"Nie istnieje użytkownik o id {Tour.UserId}" };
            }

            Tour newTour = Tour;

            Random random = new Random();
            const string chars = "abcdefghijklmnoprstuwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            newTour.InviteLink = new string(Enumerable.Repeat(chars, 20)
                .Select(s => s[random.Next(s.Length)]).ToArray());

            var response = await _TourService.CreateTour(newTour, Tour.UserId);
            if (response.Success)
                return Ok(new RepositoryResponse<int> { Data = newTour.Id, Success = true, Message = "" });
            else
                return NotFound(new RepositoryResponse<int> { Data = -1, Success = false, Message = response.Message });
        }

        [HttpPost("addParticipant")]
        public async Task<ActionResult<RepositoryResponse<bool>>> AddParticipant([FromBody] ParticipantTourDTO Tour)
        {
            var resp = await _UserService.GetUserAsync(u => u.Id == Tour.UserId);
            if (resp.Data == null)
            {
                return new RepositoryResponse<bool> { Success = false, Message = $"Nie istnieje użytkownik o id = {Tour.UserId}" };
            }
            var resp2 = await _TourService.GetTourAsync(u => u.Id == Tour.TourId, "Participants");
            if (resp2.Data == null)
            {
                return new RepositoryResponse<bool> { Success = false, Message = $"Nie istnieje wycieczka o id = {Tour.TourId}" };
            }
            var resp3 = resp2.Data.Participants.FirstOrDefault(u => u.UserId == Tour.UserId);
            if (resp3 != null)
            {
                return new RepositoryResponse<bool> { Success = false, Message = $"Wycieczka zawiera już uczestnika o UserId = {Tour.UserId}" };
            }

            ParticipantTour elem = Tour;

            var response = await _TourService.AddParticipantToTour(elem);
            if (response.Success)
                return Ok(new RepositoryResponse<bool> { Data = true, Success = true, Message = "" });
            else
                return NotFound(new RepositoryResponse<bool> { Data = false, Success = false, Message = response.Message });
        }

        [HttpGet("{TourId}/participants")]
        public async Task<ActionResult<List<ParticipantTourDTO>>> GetParticipant(int TourId)
        {
            var response = await _TourService.GetParticipantsAsync(u => u.TourId == TourId);
            return Ok(response.Data?.Select(u => (ParticipantTourDTO)u).ToList());
        }

        [HttpGet("{TourId}/participant/{userId}")]
        public async Task<ActionResult<ParticipantTourDTO>> GetParticipantById(int TourId, int userId)
        {
            var response = await _TourService.GetParticipantAsync(u => u.TourId == TourId && u.UserId == userId);
            return Ok((ParticipantTourDTO)response.Data);
        }

        [HttpPut("{TourId}/updateParticipantNickname/{userId}")]
        public async Task<ActionResult<RepositoryResponse<bool>>> UpdateParticipantNickname(int TourId, int userId, [FromBody] string newNickname)
        {
            var resp2 = await _TourService.GetTourAsync(u => u.Id == TourId, "Participants");
            if (resp2.Data == null)
            {
                return new RepositoryResponse<bool> { Success = false, Message = $"Nie istnieje wyjazd o id = {TourId}" };
            }

            var resp = await _UserService.GetUserAsync(u => u.Id == userId);
            if (resp.Data == null)
            {
                return new RepositoryResponse<bool> { Success = false, Message = $"Nie istnieje użytkownik o id = {userId}" };
            }

            ParticipantTour? participant = resp2.Data.Participants.Where(p => p.UserId == userId).Single();
            if (participant == null)
            {
                return new RepositoryResponse<bool> { Success = false, Message = $"Użytkownik nie jest uczestnikiem wycieczki" };
            }

            participant.Nickname = newNickname;

            var response = await _ParticipantTourService.UpdateParticipantTour(participant);
            if (response.Success)
                return Ok(new RepositoryResponse<bool> { Data = true, Message = "", Success = true });
            else
                return BadRequest(new RepositoryResponse<bool> { Data = false, Message = response.Message, Success = false });
        }

        [HttpDelete("{TourId}/deleteParticipant/{userId}")]
        public async Task<ActionResult<RepositoryResponse<bool>>> DeleteParticipant(int TourId, int userId)
        {
            var resp = await _UserService.GetUserAsync(u => u.Id == userId);
            if (resp.Data == null)
            {
                return new RepositoryResponse<bool> { Success = false, Message = $"Nie istnieje użytkownik o id = {userId}" };
            }
            var resp2 = await _TourService.GetTourAsync(u => u.Id == TourId);
            if (resp2.Data == null)
            {
                return new RepositoryResponse<bool> { Success = false, Message = $"Nie istnieje Wycieczka o id = {TourId}" };
            }

            ParticipantTour elem = new ParticipantTour
            {
                UserId = userId,
                TourId = TourId
            };

            var response = await _TourService.DeleteParticipantFromTour(elem);
            if (response.Success)
                return Ok(new RepositoryResponse<bool> { Data = true, Success = true, Message = "" });
            else
                return NotFound(new RepositoryResponse<bool> { Data = false, Success = false, Message = response.Message });
        }

        [HttpPut("{id}/makeOrganizer/{userId}")]
        public async Task<ActionResult<RepositoryResponse<bool>>> MakeOrganizer(int id, int userId)
        {
            var resp2 = await _TourService.GetTourAsync(u => u.Id == id, "Participants");
            if (resp2.Data == null)
            {
                return new RepositoryResponse<bool> { Success = false, Message = $"Nie istnieje Wycieczka o id = {id}" };
            }

            var resp = await _UserService.GetUserAsync(u => u.Id == userId);
            if (resp.Data == null)
            {
                return new RepositoryResponse<bool> { Success = false, Message = $"Nie istnieje użytkownik o id = {userId}" };
            }

            ParticipantTour? participant = resp2.Data.Participants.Where(p => p.UserId == userId).Single();
            if (participant == null)
            {
                return new RepositoryResponse<bool> { Success = false, Message = $"Użytkownik nie jest uczestnikiem wycieczki" };
            }

            participant.IsOrganizer = true;

            var response = await _ParticipantTourService.UpdateParticipantTour(participant);
            if (response.Success)
                return Ok(new RepositoryResponse<bool> { Data = true, Message = "", Success = true });
            else
                return BadRequest(new RepositoryResponse<bool> { Data = false, Message = response.Message, Success = false });
        }

        [HttpPut("{id}/deleteOrganizer/{userId}")]
        public async Task<ActionResult<RepositoryResponse<bool>>> DeleteOrganizer(int id, int userId)
        {
            var resp2 = await _TourService.GetTourAsync(u => u.Id == id, "Participants");
            if (resp2.Data == null)
            {
                return new RepositoryResponse<bool> { Success = false, Message = $"Nie istnieje Wycieczka o id = {id}" };
            }

            var resp = await _UserService.GetUserAsync(u => u.Id == userId);
            if (resp.Data == null)
            {
                return new RepositoryResponse<bool> { Success = false, Message = $"Nie istnieje użytkownik o id = {userId}" };
            }

            ParticipantTour? participant = resp2.Data.Participants.Where(p => p.UserId == userId).Single();
            if (participant == null)
            {
                return new RepositoryResponse<bool> { Success = false, Message = $"Użytkownik nie jest uczestnikiem wycieczki" };
            }

            participant.IsOrganizer = false;

            var response = await _ParticipantTourService.UpdateParticipantTour(participant);
            if (response.Success)
                return Ok(new RepositoryResponse<bool> { Data = true, Message = "", Success = true });
            else
                return BadRequest(new RepositoryResponse<bool> { Data = false, Message = response.Message, Success = false });
        }

        [HttpPost("addCultureAssistance")]
        public async Task<ActionResult<RepositoryResponse<bool>>> AddCultureAssistance([FromBody] CultureAssistanceDTO Tour)
        {
            var resp = await _CultureService.GetCultureAsync(u => u.Id == Tour.CultureId);
            if (resp.Data == null)
            {
                return new RepositoryResponse<bool> { Success = false, Message = $"Nie istnieje nota kulturowa o id = {Tour.CultureId}" };
            }
            var resp2 = await _TourService.GetTourAsync(u => u.Id == Tour.TourId, "Cultures");
            if (resp2.Data == null)
            {
                return new RepositoryResponse<bool> { Success = false, Message = $"Nie istnieje wycieczka o id = {Tour.TourId}" };
            }
            var resp3 = resp2.Data.Cultures.FirstOrDefault(u => u.CultureId == Tour.CultureId);
            if (resp3 != null)
            {
                return new RepositoryResponse<bool> { Success = false, Message = $"Wycieczka zawiera już note kulturowa o id = {Tour.CultureId}" };
            }

            CultureAssistance elem = Tour;

            var response = await _TourService.AddCultureAssistanceToTour(elem);
            if (response.Success)
                return Ok(new RepositoryResponse<bool> { Data = true, Message = "", Success = true });
            else
                return BadRequest(new RepositoryResponse<bool> { Data = false, Message = response.Message, Success = false });
        }

        [HttpGet("{TourId}/cultureAssistances")]
        public async Task<ActionResult<List<CultureAssistanceDTO>>> GetCulturesAssistance(int TourId)
        {
            var response = await _TourService.GetCulturesAssistanceAsync(u => u.TourId == TourId);
            return Ok(response.Data?.Select(u => (CultureAssistanceDTO)u).ToList());
        }

        [HttpGet("{TourId}/cultureAssistance/{CultureId}")]
        public async Task<ActionResult<CultureAssistanceDTO>> GetCultureAssistanceById(int TourId, int CultureId)
        {
            var response = await _TourService.GetCultureAssistanceAsync(u => u.TourId == TourId && u.CultureId == CultureId);
            return Ok((CultureAssistanceDTO)response.Data);
        }

        [HttpDelete("{TourId}/deleteCultureAssistance/{cultureId}")]
        public async Task<ActionResult<RepositoryResponse<bool>>> DeleteCultureAssistance(int TourId, int cultureId)
        {
            var resp = await _CultureService.GetCultureAsync(u => u.Id == cultureId);
            if (resp.Data == null)
            {
                return new RepositoryResponse<bool> { Success = false, Message = $"Nie istnieje nota kulturowa o id = {cultureId}" };
            }
            var resp2 = await _TourService.GetTourAsync(u => u.Id == TourId);
            if (resp2.Data == null)
            {
                return new RepositoryResponse<bool> { Success = false, Message = $"Nie istnieje Wycieczka o id = {TourId}" };
            }

            CultureAssistance elem = new CultureAssistance
            {
                CultureId = cultureId,
                TourId = TourId
            };

            var response = await _TourService.DeleteCultureAssistanceFromTour(elem);
            if (response.Success)
                return Ok(new RepositoryResponse<bool> { Data = true, Message = "", Success = true });
            else
                return BadRequest(new RepositoryResponse<bool> { Data = false, Message = response.Message, Success = false });
        }

        [HttpPut("{TourId}/updateWeatherCords")]
        public async Task<ActionResult<RepositoryResponse<bool>>> UpdateWeatherCords(int TourId, [FromBody] string newLoc)
        {
            var resp2 = await _TourService.GetTourAsync(u => u.Id == TourId);
            if (resp2.Data == null)
            {
                return new RepositoryResponse<bool> { Success = false, Message = $"Nie istnieje wyjazd o id = {TourId}" };
            }

            resp2.Data.WeatherCords = newLoc;
            
            var response = await _TourService.UpdateTour(resp2.Data);
            if (response.Success)
                return Ok(new RepositoryResponse<bool> { Data = true, Message = "", Success = true });
            else
                return BadRequest(new RepositoryResponse<bool> { Data = false, Message = response.Message, Success = false });
        }

        // PUT api/<ValuesController>/5
        [HttpPut("{id}/{userId}")]
        public async Task<ActionResult<RepositoryResponse<bool>>> Edit(int id, int userId, [FromBody] EditTourDTO Tour)
        {
            var resp2 = await _TourService.GetTourAsync(u => u.Id == id, "Participants");
            if (resp2.Data == null)
            {
                return new RepositoryResponse<bool> { Success = false, Message = $"Nie istnieje Wycieczka o id = {id}" };
            }

            var resp = await _UserService.GetUserAsync(u => u.Id == userId);
            if (resp.Data == null)
            {
                return new RepositoryResponse<bool> { Success = false, Message = $"Nie istnieje użytkownik o id = {userId}" };
            }

            Tour tour = resp2.Data;
            tour.Id = id;
            tour.MaxParticipant = Tour.MaxParticipant;
            tour.StartDate = Tour.StartDate;
            tour.EndDate = Tour.EndDate;
            tour.CreateDate = Tour.CreateDate;
            tour.TargetCountry = Tour.TargetCountry;
            tour.TargetRegion = Tour.TargetRegion;
            tour.WeatherCords = Tour.WeatherCords;
            tour.Title = Tour.Title;
            tour.Description = Tour.Description;

            var response = await _TourService.UpdateTour(tour);
            if (response.Success)
                return Ok(new RepositoryResponse<bool> { Data = true, Message = "", Success = true });
            else
                return BadRequest(new RepositoryResponse<bool> { Data = false, Message = response.Message, Success = false });
        }

        // DELETE api/<ValuesController>/5
        [HttpDelete("{tourId}/{userId}")]
        public async Task<ActionResult<RepositoryResponse<bool>>> Delete(int userId, int tourId)
        {
            var resp2 = await _TourService.GetTourAsync(u => u.Id == tourId, "Participants");
            if (resp2.Data == null)
            {
                return new RepositoryResponse<bool> { Success = false, Message = $"Nie istnieje Wycieczka o id = {tourId}" };
            }


            var resp = await _UserService.GetUserAsync(u => u.Id == userId);
            if (resp.Data == null)
            {
                return new RepositoryResponse<bool> { Success = false, Message = $"Nie istnieje użytkownik o id = {userId}" };
            }

            ParticipantTour? participant = resp2.Data.Participants.Where(p => p.UserId == resp.Data.Id).First();
            if (participant == null || participant?.IsOrganizer == false)
            {
                return new RepositoryResponse<bool> { Success = false, Message = $"Odmowa dostępu!" };
            }

            var response = await _TourService.DeleteTour(new Tour() { Id = tourId });
            if (response.Success)
                return Ok(new RepositoryResponse<bool> { Data = true, Message = "", Success = true });
            else
                return BadRequest(new RepositoryResponse<bool> { Data = false, Message = response.Message, Success = false });
        }
    }
}
