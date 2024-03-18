using TripPlanner.Models.Models.MessageModels.QuestionnaireModels;

namespace TripPlanner.Models.DTO.MessageDTOs.QuestionnaireDTOs
{
    public class CreateQuestionnaireDTO
    {
        public int UserId { get; set; }
        public int TourId { get; set; }
        public string Content { get; set; } = string.Empty;
        public ICollection<CreateQuestionnaireAnswerDTO> Answers { get; set; } = new List<CreateQuestionnaireAnswerDTO>();


        public static implicit operator Questionnaire(CreateQuestionnaireDTO data)
        {
            if (data == null)
                return null;

            return new Questionnaire
            {
                UserId = data.UserId,
                TourId = data.TourId,
                Content = data.Content,
                Answers = data.Answers.Select(u => (QuestionnaireAnswer)u).ToList()
            };
        }
    }
}
