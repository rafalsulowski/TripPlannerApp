using Microsoft.AspNetCore.Mvc;
using TripPlanner.Models;
using TripPlanner.Services.CheckListService;
using TripPlanner.Services.UserService;
using TripPlanner.Services.TourService;
using TripPlanner.Models.DTO.CheckListDTOs;
using TripPlanner.Services.ChatService;
using TripPlanner.Models.Models;
using TripPlanner.Models.Models.CheckListModels;
using TripPlanner.Models.Models.TourModels;
using static System.Runtime.InteropServices.JavaScript.JSType;
using Microsoft.AspNetCore.Authorization;

namespace TripPlanner.WebAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    //[ApiExplorerSettings(IgnoreApi = ProjectConfiguration.HideContorller)]
    public class CheckListController : ControllerBase
    {
        private readonly ICheckListService _CheckListService;
        private readonly IUserService _UserService;
        private readonly ITourService _TourService;

        public CheckListController(ICheckListService CheckListService, IUserService userService, ITourService tourService)
        {
            _CheckListService = CheckListService;
            _UserService = userService;
            _TourService = tourService;
        }

        [HttpGet]
        public async Task<ActionResult<List<CheckListDTO>>> Get()
        {
            var response = await _CheckListService.GetCheckListsAsync();
            List<CheckListDTO> res = response.Data.Select(u => (CheckListDTO)u).ToList();
            return Ok(res);
        }

        [HttpGet("getWithFields/{id}")]
        public async Task<ActionResult<CheckListDTO>> GetWithFields(int id)
        {
            var response = await _CheckListService.GetCheckListAsync(u => u.Id == id, "Fields");
            CheckListDTO res = response.Data;
            return Ok(res);
        }

        [HttpGet("getFromTour/{tourId}")]
        public async Task<ActionResult<List<CheckListDTO>>> getFromTour(int tourId)
        {
            var response = await _CheckListService.GetCheckListsAsync(u => u.TourId == tourId, "Fields");
            List<CheckListDTO> res = response.Data.Select(u => (CheckListDTO)u).ToList();
            return Ok(res);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CheckListDTO>> GetById(int id)
        {
            var response = await _CheckListService.GetCheckListAsync(u => u.Id == id);
            CheckListDTO res = response.Data;
            return Ok(res);
        }

        [HttpPost("createCheckList")]
        public async Task<ActionResult<RepositoryResponse<int>>> CreateCheckList([FromBody] CreateCheckListDTO CheckList)
        {
            var resp3 = await _TourService.GetTourAsync(u => u.Id == CheckList.TourId, "CheckLists");
            if (resp3.Data == null)
                return new RepositoryResponse<int> { Data = -1, Success = false, Message = $"Nie istnieje wycieczka o id = {CheckList.TourId}" };
            
            if (resp3.Data.CheckLists.FirstOrDefault(u => u.Name == CheckList.Name) != null)
                return new RepositoryResponse<int> { Data = -1, Success = false, Message = $"W tej wycieczce istnieje checklista o takiej nazwie = {CheckList.Name}" };
            
            CheckList newCheckList = CheckList;

            var response = await _CheckListService.CreateCheckList(newCheckList);
            if (response.Success)
                return Ok(new RepositoryResponse<int> { Success = true, Message = "", Data = newCheckList.Id });
            else
                return NotFound(new RepositoryResponse<int> { Success = false, Message = response.Message, Data = -1 });
        }

        [HttpGet("{checkListId}/checkListFields")]
        public async Task<ActionResult<List<CheckListFieldDTO>>> GetCheckListFields(int checkListId)
        {
            var response = await _CheckListService.GetCheckListFieldsAsync(u => u.CheckListId == checkListId);
            List<CheckListFieldDTO> res = response.Data.Select(u => (CheckListFieldDTO)u).ToList();
            return Ok(res);
        }

        [HttpGet("{checkListId}/checkListField/{fieldId}")]
        public async Task<ActionResult<CheckListFieldDTO>> GetCheckListFieldById(int checkListId, int fieldId)
        {
            var response = await _CheckListService.GetCheckListFieldAsync(u => u.CheckListId == checkListId && u.Id == fieldId);
            CheckListFieldDTO res = response.Data;
            return Ok(res);
        }

        [HttpPost("addCheckListField/{userId}")]
        public async Task<ActionResult<RepositoryResponse<bool>>> AddCheckListField(int userId, [FromBody] CreateCheckListFieldDTO CheckListField)
        {
            var resp = await _CheckListService.GetCheckListAsync(u => u.Id == CheckListField.CheckListId, "Fields");
            if (resp.Data == null)
                return new RepositoryResponse<bool> { Success = false, Message = $"Nie istnieje checklista o id = {CheckListField.CheckListId}" };
            
            if (resp.Data.Fields.FirstOrDefault(u => u.Name == CheckListField.Name) != null)
                return new RepositoryResponse<bool> { Success = false, Message = $"W tej checkliscie istnieje pole o takiej nazwie = {CheckListField.Name}" };
            
            CheckList checkList = resp.Data;

            if (!checkList.IsPublic)
            {
                //sprawdzenie czy jest organizatorem lub autorem checklisty
                var resp4 = await _TourService.GetTourAsync(u => u.Id == checkList.TourId, "Participants");
                if (resp4.Data == null)
                    return new RepositoryResponse<bool> { Success = false, Message = $"Nie istnieje Wycieczka o id = {checkList.TourId}" };
                
                var resp5 = await _UserService.GetUserAsync(u => u.Id == userId);
                if (resp.Data == null)
                    return new RepositoryResponse<bool> { Success = false, Message = $"Nie istnieje użytkownik o id = {userId}" };
                
                ParticipantTour? participant = resp4.Data.Participants.Where(p => p.UserId == resp.Data.Id).First();
                if (resp.Data.Id != checkList.UserId) //jesli nie jest autorem
                    if (participant == null || participant?.IsOrganizer == false) //jesli nie jest organizatorem
                        return Unauthorized("Odmowa dostępu!");
            }

            CheckListField elem = CheckListField;

            var response = await _CheckListService.AddFieldToCheckList(elem);
            if (response.Success)
                return Ok(new RepositoryResponse<bool> { Success = true, Message = "", Data = true });
            else
                return NotFound(new RepositoryResponse<bool> { Success = false, Message = response.Message, Data = false });
        }

        [HttpPut("{checkListId}/editCheckListField/{fieldId}/{userId}")]
        public async Task<ActionResult<RepositoryResponse<bool>>> EditCheckListField(int checkListId, int fieldId, int userId, [FromBody] EditCheckListFieldDTO CheckListField)
        {
            var resp2 = await _CheckListService.GetCheckListFieldsAsync(u => u.Id == fieldId);
            if (resp2.Data == null)
                return new RepositoryResponse<bool> { Success = false, Message = $"Nie istnieje pole o id = {fieldId}" };
            
            var resp = await _CheckListService.GetCheckListAsync(u => u.Id == checkListId, "Fields");
            if (resp.Data == null)
                return new RepositoryResponse<bool> { Success = false, Message = $"Nie istnieje checklista o id = {checkListId}" };
            
            var field = resp.Data.Fields.FirstOrDefault(u => u.Name == CheckListField.Name);
            if (field != null && field.Id != fieldId)
                return new RepositoryResponse<bool> { Success = false, Message = $"W tej checkliscie istnieje pole o takiej nazwie = {CheckListField.Name}" };
            
            CheckList checkList = resp.Data;

            if (!checkList.IsPublic)
            {
                //sprawdzenie czy jest organizatorem lub autorem checklisty
                var resp4 = await _TourService.GetTourAsync(u => u.Id == checkList.TourId, "Participants");
                if (resp4.Data == null)
                    return new RepositoryResponse<bool> { Success = false, Message = $"Nie istnieje Wycieczka o id = {checkList.TourId}" };
                
                var resp5 = await _UserService.GetUserAsync(u => u.Id == userId);
                if (resp.Data == null)
                    return new RepositoryResponse<bool> { Success = false, Message = $"Nie istnieje użytkownik o id = {userId}" };
                
                ParticipantTour? participant = resp4.Data.Participants.Where(p => p.UserId == resp.Data.Id).First();
                if (resp.Data.Id != checkList.UserId)
                    if (participant == null || participant?.IsOrganizer == false)
                        return Unauthorized("Odmowa dostępu!");
            }

            CheckListField CheckList2 = CheckListField;
            CheckList2.CheckListId = checkListId;
            CheckList2.Id = fieldId;

            var response = await _CheckListService.UpdateCheckListField(CheckList2);
            if (response.Success)
                return Ok(new RepositoryResponse<bool> { Success = true, Message = "", Data = true });
            else
                return NotFound(new RepositoryResponse<bool> { Success = false, Message = response.Message, Data = false });
        }

        [HttpDelete("{checkListId}/deleteCheckListField/{fieldId}/{userId}")]
        public async Task<ActionResult<RepositoryResponse<bool>>> DeleteCheckListField(int checkListId, int fieldId, int userId)
        {
            var resp2 = await _CheckListService.GetCheckListFieldAsync(u => u.Id == fieldId);
            if (resp2.Data == null)
                return new RepositoryResponse<bool> { Success = true, Message = "", Data = true }; //gdy takie pole nie istnieje
            
            var resp = await _CheckListService.GetCheckListAsync(u => u.Id == checkListId, "Fields");
            if (resp.Data == null)
                return new RepositoryResponse<bool> { Success = false, Message = $"Nie istnieje checklista o id = {checkListId}" };
            
            CheckList checkList = resp.Data;

            if (!checkList.IsPublic)
            {
                //sprawdzenie czy jest organizatorem lub autorem checklisty
                var resp4 = await _TourService.GetTourAsync(u => u.Id == checkList.TourId, "Participants");
                if (resp4.Data == null)
                    return new RepositoryResponse<bool> { Success = false, Message = $"Nie istnieje Wycieczka o id = {checkList.TourId}" };

                var resp5 = await _UserService.GetUserAsync(u => u.Id == userId);
                if (resp.Data == null)
                    return new RepositoryResponse<bool> { Success = false, Message = $"Nie istnieje użytkownik o id = {userId}" };

                ParticipantTour? participant = resp4.Data.Participants.Where(p => p.UserId == resp.Data.Id).First();
                if (resp.Data.Id != checkList.UserId)
                    if (participant == null || participant?.IsOrganizer == false)
                        return Unauthorized("Odmowa dostępu!");
            }

            CheckListField elem = resp2.Data;
            elem.CheckListId = checkListId;
            elem.Id = fieldId;

            var response = await _CheckListService.DeleteFieldFromCheckList(elem);
            if (response.Success)
                return Ok(new RepositoryResponse<bool> { Success = true, Message = "", Data = true });
            else
                return NotFound(new RepositoryResponse<bool> { Success = false, Message = response.Message, Data = false });
        }

        [HttpPut("{checkListId}/{userId}")]
        public async Task<ActionResult<RepositoryResponse<bool>>> Edit(int checkListId, int userId, [FromBody] EditCheckListDTO CheckList)
        {
            var resp3 = await _CheckListService.GetCheckListAsync(u => u.Id == checkListId);
            if (resp3.Data == null)
                return new RepositoryResponse<bool> { Success = false, Message = $"Nie istnieje checklista o id = {checkListId}" };
            
            CheckList checkList = resp3.Data;

            if(!checkList.IsPublic)
            {
                //sprawdzenie czy jest organizatorem lub autorem checklisty
                var resp2 = await _TourService.GetTourAsync(u => u.Id == checkList.TourId, "Participants");
                if (resp2.Data == null)
                    return new RepositoryResponse<bool> { Success = false, Message = $"Nie istnieje Wycieczka o id = {checkList.TourId}" };
                
                var resp = await _UserService.GetUserAsync(u => u.Id == userId);
                if (resp.Data == null)
                    return new RepositoryResponse<bool> { Success = false, Message = $"Nie istnieje użytkownik o id = {userId}" };
               
                ParticipantTour? participant = resp2.Data.Participants.Where(p => p.UserId == resp.Data.Id).First();
                if(resp.Data.Id != checkList.UserId)
                    if (participant == null || participant?.IsOrganizer == false)
                        return Unauthorized("Odmowa dostępu!");
            }

            CheckList elem = resp3.Data;
            elem.Id = checkListId;
            elem.IsPublic = CheckList.IsPublic;
            elem.Name = CheckList.Name;

            var response = await _CheckListService.UpdateCheckList(elem);
            if (response.Success)
                return Ok(new RepositoryResponse<bool> { Success = true, Message = "", Data = true });
            else
                return NotFound(new RepositoryResponse<bool> { Success = false, Message = response.Message, Data = false });
        }

        [HttpDelete("{checkListId}/{userId}")]
        public async Task<ActionResult<RepositoryResponse<bool>>> Delete(int checkListId, int userId)
        {
            var resp3 = await _CheckListService.GetCheckListAsync(u => u.Id == checkListId);
            if (resp3.Data == null)
                return new RepositoryResponse<bool> { Success = true, Message = "", Data = true };

            CheckList checkList = resp3.Data;

            if (!checkList.IsPublic)
            {
                //sprawdzenie czy jest organizatorem lub autorem checklisty
                var resp2 = await _TourService.GetTourAsync(u => u.Id == checkList.TourId, "Participants");
                if (resp2.Data == null)
                    return new RepositoryResponse<bool> { Success = false, Message = $"Nie istnieje Wycieczka o id = {checkList.TourId}" };

                var resp = await _UserService.GetUserAsync(u => u.Id == userId);
                if (resp.Data == null)
                    return new RepositoryResponse<bool> { Success = false, Message = $"Nie istnieje użytkownik o id = {userId}" };
                
                ParticipantTour? participant = resp2.Data.Participants.Where(p => p.UserId == resp.Data.Id).First();
                if (resp.Data.Id != checkList.UserId)
                    if (participant == null || participant?.IsOrganizer == false)
                        return Unauthorized("Odmowa dostępu!");
            }

            var response = await _CheckListService.DeleteCheckList(new CheckList() { Id = checkListId });
            if (response.Success)
                return Ok(new RepositoryResponse<bool> { Success = true, Message = "", Data = true });
            else
                return NotFound(new RepositoryResponse<bool> { Success = false, Message = response.Message, Data = false });
        }
    }
}
