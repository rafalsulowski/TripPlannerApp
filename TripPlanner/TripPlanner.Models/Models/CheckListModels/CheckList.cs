using TripPlanner.Models.DTO.CheckListDTOs;
using TripPlanner.Models.Models.TourModels;
using TripPlanner.Models.Models.UserModels;

namespace TripPlanner.Models.Models.CheckListModels
{
    public class CheckList
    {
        public int Id { get; set; }

        public User User { get; set; } = null!;
        public int UserId { get; set; }
        public Tour? Tour { get; set; }
        public int? TourId { get; set; }
        public ICollection<CheckListField> Fields { get; set; } = new List<CheckListField>();

        public string Name { get; set; } = string.Empty;
        public bool IsPublic { get; set; }


        public static implicit operator CheckListDTO(CheckList data)
        {
            if (data == null)
                return null;

            return new CheckListDTO
            {
                Id = data.Id,
                UserId = data.UserId,
                TourId = data.TourId,
                Fields = data.Fields.Select(u => (CheckListFieldDTO)u).ToList(),
                Name = data.Name,
                IsPublic = data.IsPublic
            };
        }
    }
}
