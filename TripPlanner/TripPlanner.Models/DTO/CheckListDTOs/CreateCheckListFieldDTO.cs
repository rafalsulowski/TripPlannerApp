using TripPlanner.Models.Models.CheckListModels;

namespace TripPlanner.Models.DTO.CheckListDTOs
{
    public class CreateCheckListFieldDTO
    {
        public int CheckListId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Multiplicity { get; set; } = string.Empty;


        public static implicit operator CheckListField(CreateCheckListFieldDTO data)
        {
            if (data == null)
                return null;

            return new CheckListField
            {
                CheckListId = data.CheckListId,
                Name = data.Name,
                Multiplicity = data.Multiplicity
            };
        }

    }
}
