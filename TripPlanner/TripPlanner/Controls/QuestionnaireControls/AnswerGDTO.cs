
using CommunityToolkit.Mvvm.ComponentModel;
using TripPlanner.Models.DTO.MessageDTOs.QuestionnaireDTOs;

namespace TripPlanner.Controls.QuestionnaireControls
{
    public class AnswerGDTO : ObservableObject
    {
        

        public int Id {  get; set; }
        public int QuestionnaireId {  get; set; }
        public DateTime Date {  get; set; }
        public string Answer {  get; set; } = string.Empty;
        
        
        public double percentageShare;
        public double PercentageShare
        {
            get => percentageShare;
            set => SetProperty(ref percentageShare, value);
        }

        public string accurateIcon;
        public string AccurateIcon
        {
            get => accurateIcon;
            set => SetProperty(ref accurateIcon, value);
        }
    }
}
