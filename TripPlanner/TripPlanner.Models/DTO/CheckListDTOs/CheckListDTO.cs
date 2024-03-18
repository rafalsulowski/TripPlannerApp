using CommunityToolkit.Mvvm.ComponentModel;
using TripPlanner.Models.Models.CheckListModels;

namespace TripPlanner.Models.DTO.CheckListDTOs
{
    public class CheckListDTO : ObservableObject
    {
        public int Id { get; set; }

        public int UserId { get; set; }
        public int? TourId { get; set; }
        public ICollection<CheckListFieldDTO> Fields { get; set; } = new List<CheckListFieldDTO>();


        bool isPublic;
        public bool IsPublic
        {
            get => isPublic;
            set => SetProperty(ref isPublic, value);
        }

        string name = string.Empty;
        public string Name
        {
            get => name;
            set => SetProperty(ref name, value);
        }


        public static implicit operator CheckList(CheckListDTO data)
        {
            if (data == null)
                return null;

            return new CheckList
            {
                Id = data.Id,
                UserId = data.UserId,
                TourId = data.TourId,
                Fields = data.Fields.Select(u => (CheckListField)u).ToList(),
                Name = data.Name,
                IsPublic = data.IsPublic
            };
        }
    }
}
