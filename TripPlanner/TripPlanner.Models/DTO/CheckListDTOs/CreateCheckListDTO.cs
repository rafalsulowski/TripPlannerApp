using System.Security.Cryptography;
using TripPlanner.Models.Models.CheckListModels;

namespace TripPlanner.Models.DTO.CheckListDTOs
{
    public class CreateCheckListDTO
    {
        public int UserId { get; set; }
        public int TourId { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool IsPublic { get; set; }
        public ICollection<CreateCheckListFieldDTO> Fields { get; set; } = new List<CreateCheckListFieldDTO>();


        public static implicit operator CheckList(CreateCheckListDTO data)
        {
            if (data == null)
                return null;

            return new CheckList
            {
                UserId = data.UserId,
                TourId = data.TourId,
                Name = data.Name,
                IsPublic = data.IsPublic,
                Fields = data.Fields.Select(u => (CheckListField)u).ToList()
            };
        }
    }
}
