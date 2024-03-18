using TripPlanner.Models.Models.CheckListModels;

namespace TripPlanner.Models.DTO.CheckListDTOs
{
    public class EditCheckListDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool IsPublic { get; set; }


        public static implicit operator CheckList(EditCheckListDTO data)
        {
            if (data == null)
                return null;

            return new CheckList
            {
                Id = data.Id,
                Name = data.Name,
                IsPublic = data.IsPublic
            };
        }
    }
}
