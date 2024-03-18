using TripPlanner.Models.Models.MessageModels.QuestionnaireModels;

namespace TripPlanner.Models.DTO.MessageDTOs.QuestionnaireDTOs
{
    public class EditQuestionnaireAnswerDTO
    {
        public string Answer { get; set; } = string.Empty;


        public static implicit operator QuestionnaireAnswer(EditQuestionnaireAnswerDTO data)
        {
            if (data == null)
                return null;

            return new QuestionnaireAnswer
            {
                Answer = data.Answer
            };
        }
    }
}
