using TripPlanner.Models.Models.MessageModels.QuestionnaireModels;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Text.Json.Serialization;

namespace TripPlanner.Models.DTO.MessageDTOs.QuestionnaireDTOs
{
    public class QuestionnaireAnswerDTO : ObservableObject
    {
        public int Id { get; set; }

        public int QuestionnaireId { get; set; }


        private ICollection<QuestionnaireVoteDTO> votes = new List<QuestionnaireVoteDTO>();
        public ICollection<QuestionnaireVoteDTO> Votes
        {
            get => votes;
            set => SetProperty(ref votes, value);
        }

        public string Answer { get; set; } = string.Empty;

        public double share;
        public double Share
        {
            get => share;
            set => SetProperty(ref share, value);
        }


        public static implicit operator QuestionnaireAnswer(QuestionnaireAnswerDTO data)
        {
            if (data == null)
                return null;

            return new QuestionnaireAnswer
            {
                Id = data.Id,
                QuestionnaireId = data.QuestionnaireId,
                Votes = data.Votes.Select(u => (QuestionnaireVote)u).ToList(),
                Answer = data.Answer,
                Share = data.Share
            };
        }
    }
}
