using TripPlanner.Models.Models.MessageModels.QuestionnaireModels;

namespace TripPlanner.Models.DTO.MessageDTOs.QuestionnaireDTOs
{
    public class CreateQuestionnaireVoteDTO
    {
        public int QuestionnaireId { get; set; }
        public int AnswerId { get; set; }
        public int TourId { get; set; }
        public int UserId { get; set; }
    }
}
