using System.Text.Json.Serialization;
using TripPlanner.Models.Models.MessageModels.QuestionnaireModels;

namespace TripPlanner.Models.DTO.MessageDTOs.QuestionnaireDTOs
{
    public class QuestionnaireDTO : MessageDTO
    {
        [JsonIgnore]
        private ICollection<QuestionnaireAnswerDTO> answers = new List<QuestionnaireAnswerDTO>();
        public ICollection<QuestionnaireAnswerDTO> Answers
        {
            get => answers;
            set => SetProperty(ref answers, value);
        }


        private string voteForLabel = "Nie oddano głosu";
        public string VoteForLabel
        {
            get => voteForLabel;
            set => SetProperty(ref voteForLabel, value);
        }

        public override Questionnaire MapFromDTO()
        {
            return new Questionnaire
            {
                Id = Id,
                UserId = UserId,
                TourId = TourId,
                Content = Content,
                Answers = Answers.Select(u => (QuestionnaireAnswer)u).ToList(),
                Date = Date,
                VoteForLabel = VoteForLabel,
            };
        }
    }
}
