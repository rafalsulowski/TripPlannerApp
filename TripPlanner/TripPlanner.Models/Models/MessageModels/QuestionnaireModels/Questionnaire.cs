using TripPlanner.Models.DTO.MessageDTOs;
using TripPlanner.Models.DTO.MessageDTOs.QuestionnaireDTOs;
using TripPlanner.Models.Models.TourModels;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TripPlanner.Models.Models.MessageModels.QuestionnaireModels
{
    public class Questionnaire : Message
    {
        public ICollection<QuestionnaireAnswer> Answers { get; set; } = new List<QuestionnaireAnswer>();
        public string VoteForLabel { get; set; } = "Nie oddano głosu";

        public override QuestionnaireDTO MapToDTO()
        {
            return new QuestionnaireDTO
            {
                Id = Id,
                UserId = UserId,
                TourId = TourId,
                Content = Content,
                Answers = Answers.Select(u => (QuestionnaireAnswerDTO)u).ToList(),
                Date = Date,
                VoteForLabel = VoteForLabel,
            };
        }
    }
}
