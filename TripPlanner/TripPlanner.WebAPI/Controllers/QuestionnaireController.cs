using Microsoft.AspNetCore.Mvc;
using TripPlanner.Models;
using TripPlanner.Services.QuestionnaireService;
using TripPlanner.Services.UserService;
using TripPlanner.Services.TourService;
using TripPlanner.Models.Models;
using TripPlanner.Models.DTO.MessageDTOs.QuestionnaireDTOs;
using TripPlanner.Models.Models.MessageModels.QuestionnaireModels;
using TripPlanner.Services.ChatService;
using TripPlanner.Models.DTO.TourDTOs;
using Microsoft.AspNetCore.Authorization;

namespace TripPlanner.WebAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class QuestionnaireController : ControllerBase
    {
        private readonly IQuestionnaireService _QuestionnaireService;
        private readonly IChatService _ChatService;
        private readonly IUserService _UserService;
        private readonly ITourService _TourService;

        public QuestionnaireController(IQuestionnaireService QuestionnaireService, IUserService userService, ITourService tourService, IChatService chatService)
        {
            _QuestionnaireService = QuestionnaireService;
            _UserService = userService;
            _TourService = tourService;
            _ChatService = chatService;
        }

        [HttpGet]
        public async Task<ActionResult<List<QuestionnaireDTO>>> Get()
        {
            var response = await _QuestionnaireService.GetQuestionnairesAsync();
            List<QuestionnaireDTO> res = response.Data != null ? response.Data.Select(u => u.MapToDTO()).ToList() : new List<QuestionnaireDTO>();
            return Ok(res);
        }

        [HttpGet("getWithAnswer/{id}")]
        public async Task<ActionResult<QuestionnaireDTO>> GetWithAnswer(int id)
        {
            var response = await _QuestionnaireService.GetQuestionnaireAsync(u => u.Id == id, "Answers");
            QuestionnaireDTO? res = response.Data?.MapToDTO();
            return Ok(res);
        }

        [HttpGet("getWithAnswerAndVote/{id}")]
        public async Task<ActionResult<QuestionnaireDTO>> GetWithAnswerAndVote(int id)
        {
            var response = await _QuestionnaireService.GetQuestionnaireAsync(u => u.Id == id, "Answers.Votes");
            QuestionnaireDTO? res = response.Data?.MapToDTO();
            return Ok(res);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<QuestionnaireDTO>> GetById(int id)
        {
            var response = await _QuestionnaireService.GetQuestionnaireAsync(u => u.Id == id);
            QuestionnaireDTO? res = response.Data?.MapToDTO();
            return Ok(res);
        }

        [HttpGet("getFromTour/{tourId}")]
        public async Task<ActionResult<List<QuestionnaireDTO>>> GetFromTour(int tourId)
        {
            var response = await _QuestionnaireService.GetQuestionnairesAsync(u => u.TourId == tourId, "Answers.Votes");
            List<QuestionnaireDTO> res = response.Data != null ? response.Data.Select(u => u.MapToDTO()).ToList() : new List<QuestionnaireDTO>();
            return Ok(res);
        }

        [HttpPost("create")]
        public async Task<ActionResult<RepositoryResponse<bool>>> Create([FromBody] CreateQuestionnaireDTO Questionnaire)
        {
            var resp = await _TourService.GetTourAsync(u => u.Id == Questionnaire.TourId);
            if (resp.Data == null)
            {
                return new RepositoryResponse<bool> { Data = false, Success = false, Message = $"Nie istnieje wycieczka o id = {Questionnaire.TourId}" };
            }
            var resp2 = await _UserService.GetUserAsync(u => u.Id == Questionnaire.UserId);
            if (resp2.Data == null)
            {
                return new RepositoryResponse<bool> { Data = false, Success = false, Message = $"Nie istnieje użytkownik o id = {Questionnaire.UserId}" };
            }

            Questionnaire newQuestionnaire = Questionnaire;
            newQuestionnaire.Date = DateTime.Now;

            var response = await _QuestionnaireService.CreateQuestionnaire(newQuestionnaire);
            if(response.Success)
                return new RepositoryResponse<bool> { Data = true, Success = true, Message = "" };
            else
                return new RepositoryResponse<bool> { Data = false, Success = false, Message = response.Message };
        }

        [HttpGet("{questionnaireId}/answers")]
        public async Task<ActionResult<List<QuestionnaireAnswerDTO>>> GetAnswers(int questionnaireId)
        {
            var response = await _QuestionnaireService.GetAnswersAsync(u => u.QuestionnaireId == questionnaireId);
            List<QuestionnaireAnswerDTO> res = response.Data != null ? response.Data.Select(u => (QuestionnaireAnswerDTO)u).ToList() : new List<QuestionnaireAnswerDTO>();
            return Ok(res);
        }

        [HttpGet("{questionnaireId}/answer/{answerId}")]
        public async Task<ActionResult<QuestionnaireAnswerDTO>> GetAnswerById(int questionnaireId, int answerId)
        {
            var response = await _QuestionnaireService.GetAnswerAsync(u => u.QuestionnaireId == questionnaireId && u.Id == answerId);
            QuestionnaireAnswerDTO res = response.Data;
            return Ok(res);
        }

        [HttpGet("{questionnaireId}/getAnswerWithVote/{answerId}")]
        public async Task<ActionResult<QuestionnaireAnswerDTO>> GetAnswerWithVote(int questionnaireId, int answerId)
        {
            var response = await _QuestionnaireService.GetAnswerAsync(u => u.QuestionnaireId == questionnaireId && u.Id == answerId, "Votes");
            QuestionnaireAnswerDTO res = response.Data;
            return Ok(res);
        }

        [HttpPost("addAnswer")]
        public async Task<ActionResult<RepositoryResponse<bool>>> AddAnswer([FromBody] CreateQuestionnaireAnswerDTO Questionnaire)
        {
            var resp2 = await _QuestionnaireService.GetQuestionnaireAsync(u => u.Id == Questionnaire.QuestionnaireId, "Answers");
            if (resp2.Data == null)
            {
                return new RepositoryResponse<bool> { Success = false, Message = $"Nie istnieje Ankieta o id = {Questionnaire.QuestionnaireId}" };
            }
            var res = resp2.Data.Answers.FirstOrDefault(u => u.Answer == Questionnaire.Answer);
            if (res != null)
            {
                return new RepositoryResponse<bool> { Success = false, Message = $"Ankieta zawiera już odpowiedz o tresci = {Questionnaire.Answer}" };
            }

            QuestionnaireAnswer elem = Questionnaire;

            var response = await _QuestionnaireService.AddAnswerToQuestionnaire(elem);
            return Ok(response.Data);
        }

        [HttpPut("{questionnaireId}/editAnswer/{AnswerId}")]
        public async Task<ActionResult<RepositoryResponse<bool>>> Edit(int questionnaireId, int AnswerId, [FromBody] EditQuestionnaireAnswerDTO Questionnaire)
        {
            var resp3 = await _QuestionnaireService.GetAnswerAsync(u => u.Id == AnswerId);
            if (resp3.Data == null)
            {
                return new RepositoryResponse<bool> { Success = false, Message = $"Nie istnieje odpowiedz o id = {AnswerId}" };
            }
            var resp2 = await _QuestionnaireService.GetQuestionnaireAsync(u => u.Id == questionnaireId, "Answers");
            if (resp2.Data == null)
            {
                return new RepositoryResponse<bool> { Success = false, Message = $"Nie istnieje Ankieta o id = {questionnaireId}" };
            }
            var res = resp2.Data.Answers.FirstOrDefault(u => u.Answer == Questionnaire.Answer && u.Id != AnswerId);
            if (res != null)
            {
                return new RepositoryResponse<bool> { Success = false, Message = $"Ankieta zawiera już odpowiedz o tresci = {Questionnaire.Answer}" };
            }

            QuestionnaireAnswer newQuestionnaireAnswer = Questionnaire;
            newQuestionnaireAnswer.QuestionnaireId = questionnaireId;
            newQuestionnaireAnswer.Id = AnswerId;

            var response = await _QuestionnaireService.UpdateAnswer(newQuestionnaireAnswer);
            return Ok(response.Data);
        }

        [HttpDelete("{questionnaireId}/deleteAnswer/{answerId}")]
        public async Task<ActionResult<RepositoryResponse<bool>>> DeleteAnswer(int questionnaireId, int answerId)
        {
            var resp3 = await _QuestionnaireService.GetAnswerAsync(u => u.Id == answerId);
            if (resp3.Data == null)
            {
                return new RepositoryResponse<bool> { Success = false, Message = $"Nie istnieje odpowiedz o id = {answerId}" };
            }
            var resp2 = await _QuestionnaireService.GetQuestionnaireAsync(u => u.Id == questionnaireId);
            if (resp2.Data == null)
            {
                return new RepositoryResponse<bool> { Success = false, Message = $"Nie istnieje Ankieta o id = {questionnaireId}" };
            }

            QuestionnaireAnswer elem = resp3.Data;
            elem.Id = answerId;
            elem.QuestionnaireId = questionnaireId;
            
            var response = await _QuestionnaireService.DeleteAnswerFromQuestionnaire(elem);
            return Ok(response.Data);
        }

        [HttpPost("addVote")]
        public async Task<ActionResult<RepositoryResponse<bool>>> AddVote([FromBody] CreateQuestionnaireVoteDTO QuestionnaireVote)
        {
            var resp = await _QuestionnaireService.GetAnswerAsync(u => u.Id == QuestionnaireVote.AnswerId, "Votes");
            if (resp.Data == null)
            {
                return new RepositoryResponse<bool> { Success = false, Message = $"Nie istnieje odpowiedz o id = {QuestionnaireVote.AnswerId}" };
            }
            var resp2 = await _UserService.GetUserAsync(u => u.Id == QuestionnaireVote.UserId);
            if (resp2.Data == null)
            {
                return new RepositoryResponse<bool> { Success = false, Message = $"Nie istnieje użytkownik o id = {QuestionnaireVote.UserId}" };
            }

            CreateQuestionnaireVoteDTO vote = QuestionnaireVote;

            var response = await _QuestionnaireService.AddVoteToAnswer(vote);
            return Ok(response.Data);
        }

        [HttpGet("{questionnaireAnswerId}/votes/{tourId}")]
        public async Task<ActionResult<List<string>>> GetVotes(int questionnaireAnswerId, int tourId)
        {
            var response = await _QuestionnaireService.GetVotesAsync(u => u.QuestionnaireAnswerId == questionnaireAnswerId);
            List<QuestionnaireVoteDTO> res = response.Data.Select(u => (QuestionnaireVoteDTO)u).ToList();

            List<string> result = new List<string>();
            var participants = await _TourService.GetTourExtendParticipants(tourId);
            if (participants == null)
            {
                return Ok(result);
            }

            foreach (QuestionnaireVote vote in response.Data)
            {
                var participant = participants.Data.FirstOrDefault(u => u.UserId == vote.UserId);

                if (participant != null)
                {
                    result.Add(string.IsNullOrEmpty(participant.Nickname) ? participant.FullName : participant.Nickname);
                }
            }

            return Ok(result);
        }

        [HttpGet("{questionnaireId}/vote/{UserId}")]
        public async Task<ActionResult<QuestionnaireVoteDTO>> GetVoteById(int questionnaireId, int UserId)
        {
            var response = await _QuestionnaireService.GetVoteAsync(u => u.QuestionnaireAnswerId == questionnaireId && u.UserId == UserId);
            QuestionnaireVoteDTO res = response.Data;
            return Ok(res);
        }

        [HttpDelete("{questionnaireAnswerId}/deleteVote/{UserId}")]
        public async Task<ActionResult<RepositoryResponse<bool>>> DeleteVote(int questionnaireAnswerId, int UserId)
        {
            var resp = await _QuestionnaireService.GetVoteAsync(u => u.UserId == UserId && u.QuestionnaireAnswerId == questionnaireAnswerId);
            if (resp.Data == null)
            {
                return new RepositoryResponse<bool> { Success = false, Message = $"Nie istnieje glos o id = {UserId}" };
            }
            var resp2 = await _QuestionnaireService.GetAnswerAsync(u => u.Id == questionnaireAnswerId);
            if (resp2.Data == null)
            {
                return new RepositoryResponse<bool> { Success = false, Message = $"Nie istnieje odpowiedz o id = {questionnaireAnswerId}" };
            }

            QuestionnaireVote elem = resp.Data;
            elem.UserId = UserId;
            elem.QuestionnaireAnswerId = questionnaireAnswerId;

            var response = await _QuestionnaireService.DeleteVoteFromAnswer(elem);
            return Ok(response.Data);
        }

        [HttpPut("{questionnaireId}")]
        public async Task<ActionResult<RepositoryResponse<bool>>> Edit(int questionnaireId, [FromBody] EditQuestionnaireDTO Questionnaire)
        {
            var resp2 = await _QuestionnaireService.GetQuestionnaireAsync(u => u.Id == questionnaireId);
            if (resp2.Data == null)
            {
                return new RepositoryResponse<bool> { Success = false, Message = $"Nie istnieje ankieta o id = {questionnaireId}" };
            }
            var resp = await _TourService.GetTourAsync(u => u.Id == resp2.Data.TourId, "Questionnaires");
            if (resp.Data == null)
            {
                return new RepositoryResponse<bool> { Success = false, Message = $"Nie istnieje wycieczka o id = {resp2.Data.TourId}" };
            }
            
            Questionnaire elem = resp2.Data;
            elem.Id = questionnaireId;
            elem.Content = Questionnaire.Content;

            var response = await _QuestionnaireService.UpdateQuestionnaire(elem);
            return Ok(response.Data);
        }

        [HttpDelete("{questionnaireId}")]
        public async Task<ActionResult<RepositoryResponse<bool>>> Delete(int questionnaireId)
        {
            var resp2 = await _QuestionnaireService.GetQuestionnaireAsync(u => u.Id == questionnaireId);
            if (resp2.Data == null)
            {
                return Ok(new RepositoryResponse<bool> { Success = true, Message = "", Data = true }); //gdy ankieta nie istnieje
            }

            var response = await _QuestionnaireService.DeleteQuestionnaire(new Questionnaire() { Id = questionnaireId });
            if (response.Success)
                return Ok(new RepositoryResponse<bool> { Success = true, Message = "", Data = true });
            else
                return NotFound(new RepositoryResponse<bool> { Success = false, Message = response.Message, Data = false });
        }
    }
}
