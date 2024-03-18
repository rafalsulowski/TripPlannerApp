using TripPlanner.Models.Models.CheckListModels;

namespace TripPlanner.Models.DTO.CheckListDTOs
{
    public class EditCheckListFieldDTO
    {
        public int Id { get; set; }
        public int CheckListId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Multiplicity { get; set; } = string.Empty;
        public bool IsChecked { get; set; }


        public static implicit operator CheckListField(EditCheckListFieldDTO data)
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
