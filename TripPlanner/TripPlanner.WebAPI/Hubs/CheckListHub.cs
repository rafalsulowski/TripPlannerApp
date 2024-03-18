using Microsoft.AspNetCore.SignalR;
using TripPlanner.Models.DTO.MessageDTOs;
using Newtonsoft.Json;
using TripPlanner.Models.Models.MessageModels;
using TripPlanner.Services.TourService;
using TripPlanner.Services.UserService;
using TripPlanner.Services.ChatService;
using TripPlanner.Services.QuestionnaireService;
using TripPlanner.Models.Models.MessageModels.QuestionnaireModels;
using TripPlanner.Models.DTO.MessageDTOs.QuestionnaireDTOs;
using TripPlanner.Models.DTO.TourDTOs;
using System.Collections.Immutable;
using TripPlanner.Services.CheckListService;
using TripPlanner.Models.Models.CheckListModels;
using TripPlanner.Models.DTO.CheckListDTOs;

namespace TripPlanner.WebAPI.Hubs
{
    public class CheckListHub : Hub
    {
        private readonly ICheckListService _CheckListService;
        public CheckListHub(ITourService tourService, IUserService userService, ICheckListService checkListService)
        {
            _CheckListService = checkListService;
        }

        public Task JoinGroup(string groupName)
        {
            return Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        }

        public Task LeaveGroup(string groupName)
        {
            return Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
        }


        public async Task CheckListDelete(string checkListId)
        {
            int id = int.Parse(checkListId);
            var res = await _CheckListService.GetCheckListAsync(u => u.Id == id, "Fields");

            if (res.Data != null)
            {
                var response = await _CheckListService.DeleteCheckList(res.Data);
                if (!response.Success)
                    throw new HubException(response.Message);
            }

            await Clients.Group("CheckList" + id.ToString()).SendAsync("CheckListDeleteReceived", id.ToString());
        }

        public async Task CheckListChangeVisibility(string json)
        {
            EditCheckListDTO checkList = JsonConvert.DeserializeObject<EditCheckListDTO>(json);
            if (checkList == null)
                throw new HubException($"Nie udało się deserializować informacji");

            var res = await _CheckListService.UpdateCheckList(checkList);
            if (checkList.IsPublic)
                await Clients.Group("CheckList" + checkList.Id.ToString()).SendAsync("CheckListChangeVisibilityToPublicReceived", checkList.Id.ToString());
            else
                await Clients.Group("CheckList" + checkList.Id.ToString()).SendAsync("CheckListChangeVisibilityToPrivateReceived", checkList.Id.ToString());
        }

        public async Task CheckListChangeName(string json)
        {
            EditCheckListDTO checkList = JsonConvert.DeserializeObject<EditCheckListDTO>(json);
            if (checkList == null)
                throw new HubException($"Nie udało się deserializować informacji");

            string name = "";
            var res = await _CheckListService.UpdateCheckList(checkList);
            if (res.Success)
                name = checkList.Name;
            else
                throw new HubException(res.Message);

            if (checkList.IsPublic)
                await Clients.Group("CheckList" + checkList.Id.ToString()).SendAsync("CheckListChangeNameReceived", name);
            else
                await Clients.Caller.SendAsync("CheckListChangeNameReceived", name);
        }

        public async Task CheckListFieldDelete(string checkListFieldId)
        {
            int id = int.Parse(checkListFieldId);
            var res = await _CheckListService.GetCheckListFieldAsync(u => u.Id == id);

            int ret = -1;
            if (res.Success && res.Data != null)
            {
                var resp = await _CheckListService.DeleteFieldFromCheckList(res.Data);
                if (resp.Success)
                    ret = id;
            }

            await Clients.Group("CheckList" + res.Data?.CheckListId.ToString()).SendAsync("CheckListFieldDeleteReceived", ret.ToString());
        }

        public async Task CheckListFieldAdd(string json)
        {
            CreateCheckListFieldDTO checkList = JsonConvert.DeserializeObject<CreateCheckListFieldDTO>(json);
            if (checkList == null)
                throw new HubException($"Nie udało się deserializować informacji");

            var response = await _CheckListService.AddFieldToCheckList(checkList);
            CheckListFieldDTO field = new CheckListFieldDTO { Id = -1 };
            if (response.Success && response.Data != -1)
            {
                var response2 = await _CheckListService.GetCheckListFieldAsync(u => u.Id == response.Data);
                if (response2.Success && response2.Data != null)
                    field = response2.Data;
            }

            string json2 = JsonConvert.SerializeObject(field);
            await Clients.Group("CheckList" + checkList.CheckListId.ToString()).SendAsync("CheckListFieldAddReceived", json2);
        }

        public async Task CheckListFieldCheck(string json)
        {
            EditCheckListFieldDTO checkList = JsonConvert.DeserializeObject<EditCheckListFieldDTO>(json);
            if (checkList == null)
                throw new HubException($"Nie udało się deserializować informacji");

            var res = await _CheckListService.UpdateCheckListField(checkList);

            CheckListFieldDTO checkListFieldDTO = new CheckListFieldDTO { Id = -1 };
            if (res.Success)
            {
                var res2 = await _CheckListService.GetCheckListFieldAsync(u => u.Id == checkList.Id);

                if (res2.Success && res2.Data != null)
                    checkListFieldDTO = res2.Data;
            }

            string json2 = JsonConvert.SerializeObject(checkListFieldDTO);
            await Clients.Group("CheckList" + checkList.CheckListId.ToString()).SendAsync("CheckListFieldCheckReceived", json2);
        }


        public async Task SetConnection(string checkListId)
        {
            try
            {
                int id = int.Parse(checkListId);
                var res = await _CheckListService.GetCheckListAsync(u => u.Id == id, "Fields");

                CheckListDTO checkListDto = new CheckListDTO();
                if (res.Success && res.Data != null)
                    checkListDto = res.Data;
                else
                    throw new Exception("Nie istanieje checklista o id = " + checkListId);

                await Groups.AddToGroupAsync(Context.ConnectionId, "CheckList" + checkListId);

                string json = JsonConvert.SerializeObject(checkListDto);
                await Clients.Caller.SendAsync("SetConnection", json);
            }
            catch (Exception)
            {
                throw new HubException($"Błędny argument: {checkListId}");
            }
        }
    }
}
