
using TripPlanner.Models.DTO.BillDTOs;
using TripPlanner.Models.Models.UserModels;

namespace TripPlanner.Models.Models.BillModels
{
    public enum BillType
    {
        Equally = 0,
        Unequally = 1,
        ByPercentages = 2,
        ByShares = 3,
        ByAdjustment = 4
    }

    public class Bill : Share
    {
        public int PayerId { get; set; }
        public User Payer { get; set; } = null!;

        public BillType BillType { get; set; }
        public ICollection<BillContributor> Contributors { get; set; } = new List<BillContributor>();


        public static implicit operator BillDTO(Bill data)
        {
            if (data == null)
                return null;

            return new BillDTO
            {
                Id = data.Id,
                TourId = data.TourId,
                Name = data.Name,
                Description = data.Description,
                CreatedDate = data.CreatedDate,
                CreatorId = data.CreatorId,
                ImageFilePath = data.ImageFilePath,
                Value = data.Value,
                PayerId = data.PayerId,
                BillType = data.BillType,
                Contributors = data.Contributors.Select(u => (BillContributorDTO)u).ToList(),
            };
        }
    }
}
