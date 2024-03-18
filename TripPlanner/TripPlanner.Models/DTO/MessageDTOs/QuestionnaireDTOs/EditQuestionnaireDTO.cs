using TripPlanner.Models.Models.MessageModels.QuestionnaireModels;

namespace TripPlanner.Models.DTO.MessageDTOs.QuestionnaireDTOs
{
    public class EditQuestionnaireDTO
    {
        public string Content { get; set; } = string.Empty;


        public static implicit operator Questionnaire(EditQuestionnaireDTO data)
        {
            if (data == null)
                return null;

            return new Questionnaire
            {
                Content = data.Content,
            };
        }
    }
}
