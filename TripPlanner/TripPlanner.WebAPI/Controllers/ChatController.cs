using Microsoft.AspNetCore.Mvc;
using TripPlanner.Models;
using TripPlanner.Services.QuestionnaireService;
using TripPlanner.Services.UserService;
using TripPlanner.Services.TourService;
using TripPlanner.Models.Models;
using TripPlanner.Models.DTO.MessageDTOs.QuestionnaireDTOs;
using TripPlanner.Models.Models.MessageModels.QuestionnaireModels;
using TripPlanner.Services.ChatService;
using TripPlanner.Models.DTO.MessageDTOs;
using TripPlanner.Models.Models.MessageModels;
using Microsoft.AspNetCore.Authorization;

namespace TripPlanner.WebAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    //[ApiExplorerSettings(IgnoreApi = ProjectConfiguration.HideContorller)]
    public class ChatController : ControllerBase
    {
        private readonly IQuestionnaireService _QuestionnaireService;
        private readonly IChatService _ChatService;
        private readonly IUserService _UserService;
        private readonly ITourService _TourService;

        public ChatController(IQuestionnaireService QuestionnaireService, IUserService userService, ITourService tourService, IChatService chatService)
        {
            _QuestionnaireService = QuestionnaireService;
            _UserService = userService;
            _TourService = tourService;
            _ChatService = chatService;
        }

        [HttpPost("addTextMessage")]
        public async Task<ActionResult<RepositoryResponse<int>>> AddTextMessage([FromBody] CreateTextMessageDTO Message)
        {
            var resp = await _TourService.GetTourAsync(u => u.Id == Message.TourId);
            if (resp.Data == null)
            {
                return new RepositoryResponse<int> { Data = -1, Success = false, Message = $"Nie istnieje wycieczka o id = {Message.TourId}" };
            }
            var resp2 = await _UserService.GetUserAsync(u => u.Id == Message.UserId);
            if (resp2.Data == null)
            {
                return new RepositoryResponse<int> { Data = -1, Success = false, Message = $"Nie istnieje użytkownik o id = {Message.UserId}" };
            }
            
            TextMessage newMessage = Message;
            newMessage.Date = DateTime.Now;

            var response = await _ChatService.AddTextMessage(newMessage);
            if (response.Success)
                return new RepositoryResponse<int> { Data = newMessage.Id, Success = true, Message = $"" };
            else
                return new RepositoryResponse<int> { Data = newMessage.Id, Success = false, Message = $"Nie udało się wysłać wiadomości!" };
        }

        [HttpPost("addNoticeMessage")]
        public async Task<ActionResult<RepositoryResponse<int>>> AddNoticeMessage([FromBody] CreateNoticeMessageDTO Message)
        {
            var resp = await _TourService.GetTourAsync(u => u.Id == Message.TourId);
            if (resp.Data == null)
            {
                return new RepositoryResponse<int> { Data = -1, Success = false, Message = $"Nie istnieje wycieczka o id = {Message.TourId}" };
            }
            var resp2 = await _UserService.GetUserAsync(u => u.Id == Message.UserId);
            if (resp2.Data == null)
            {
                return new RepositoryResponse<int> { Data = -1, Success = false, Message = $"Nie istnieje użytkownik o id = {Message.UserId}" };
            }

            NoticeMessage newMessage = Message;
            newMessage.Date = DateTime.Now;

            var response = await _ChatService.AddNoticeMessage(newMessage);
            if (response.Success)
                return new RepositoryResponse<int> { Data = newMessage.Id, Success = true, Message = $"" };
            else
                return new RepositoryResponse<int> { Data = newMessage.Id, Success = false, Message = $"Nie udało się wysłać wiadomości!" };
        }
    }
}
