using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TripPlanner.Models.Models.BillModels;

namespace TripPlanner.Models.DTO.BillDTOs
{
    public class SharePresentationDTO
    {
        public enum ShareType
        {
            Bill,
            Transfer,
        }

        public int Id { get; set; }

        public DateTime CreatedDate { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Value { get; set; } //ile uzytkownik requestujacy zalega lub pozyczyl
        public ShareType Type { get; set; }
    }
}
