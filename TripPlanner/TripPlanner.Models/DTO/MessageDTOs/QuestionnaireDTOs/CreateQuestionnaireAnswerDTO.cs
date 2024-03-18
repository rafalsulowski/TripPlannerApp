using TripPlanner.Models.Models.MessageModels.QuestionnaireModels;

namespace TripPlanner.Models.DTO.MessageDTOs.QuestionnaireDTOs
{
    public class CreateQuestionnaireAnswerDTO
    {
        public int QuestionnaireId { get; set; }
        public string Answer { get; set; } = string.Empty;


        public static implicit operator QuestionnaireAnswer(CreateQuestionnaireAnswerDTO data)
        {
            if (data == null)
                return null;

            return new QuestionnaireAnswer
            {
                Share = 0.0,
                QuestionnaireId = data.QuestionnaireId,
                Answer = data.Answer
            };
        }
    }
}
