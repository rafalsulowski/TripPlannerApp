using CommunityToolkit.Mvvm.ComponentModel;
using TripPlanner.Models.DTO.MessageDTOs.QuestionnaireDTOs;
using TripPlanner.Models.Models.CheckListModels;

namespace TripPlanner.Models.DTO.CheckListDTOs
{
    public class CheckListFieldDTO : ObservableObject
    {
        public int Id { get; set; }
        public int CheckListId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Multiplicity { get; set; } = string.Empty;

        bool isChecked;
        public bool IsChecked
        {
            get => isChecked;
            set => SetProperty(ref isChecked, value);
        }


        public static implicit operator CheckListField(CheckListFieldDTO data)
        {
            if (data == null)
                return null;

            return new CheckListField
            {
                Id = data.Id,
                CheckListId = data.CheckListId,
                Name = data.Name,
                Multiplicity = data.Multiplicity,
                IsChecked = data.IsChecked
            };
        }

    }
}
