using TripPlanner.Models.Models.BillModels;

namespace TripPlanner.Models.DTO.BillDTOs
{
    public class BillDTO : ShareDTO
    {
        public int PayerId { get; set; }

        public BillType BillType { get; set; }
        public ICollection<BillContributorDTO> Contributors { get; set; } = new List<BillContributorDTO>();

        public static implicit operator Bill(BillDTO data)
        {
            if (data == null)
                return null;

            return new Bill
            {
                Id = data.Id,
                Description = data.Description,
                Name = data.Name,
                CreatedDate = data.CreatedDate,
                CreatorId = data.CreatorId,
                ImageFilePath = data.ImageFilePath,
                Value = data.Value,
                PayerId = data.PayerId,
                BillType = data.BillType,
                Contributors = data.Contributors.Select(u => (BillContributor)u).ToList(),
            };
        }
    }
}
