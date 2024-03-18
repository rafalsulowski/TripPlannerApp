using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TripPlanner.Models.Models.TourModels;
using TripPlanner.Models.Models;
using TripPlanner.Models.Models.BillModels;

namespace TripPlanner.Models.DTO.BillDTOs
{
    public class CreateBillDTO
    {
        public int TourId { get; set; }
        public int CreatorId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public decimal Value { get; set; }
        public string ImageFilePath { get; set; } = string.Empty;
        public int PayerId { get; set; }
        public BillType BillType { get; set; }
        public ICollection<BillContributorDTO> Contributors { get; set; } = new List<BillContributorDTO>();


        public static implicit operator Bill(CreateBillDTO data)
        {
            if (data == null)
                return null;

            return new Bill
            {
                CreatedDate = data.CreatedDate,
                Description = data.Description,
                Name = data.Name,
                CreatorId = data.CreatorId,
                ImageFilePath = data.ImageFilePath,
                Value = data.Value,
                PayerId = data.PayerId,
                BillType = data.BillType,
                Contributors = data.Contributors.Select(u => (BillContributor)u).ToList(),
                TourId = data.TourId,
            };
        }
    }
}
