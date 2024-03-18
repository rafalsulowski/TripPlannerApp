using TripPlanner.Models.DTO.CultureDTOs;

namespace TripPlanner.Models.Models.CultureModels
{
    public class Culture
    {
        public int Id { get; set; }
        public ICollection<CultureAssistance> CultureAssistances { get; set; } = new List<CultureAssistance>();

        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string Religion { get; set; } = string.Empty;
        public string Goverment { get; set; } = string.Empty;
        public string GeograpInformation { get; set; } = string.Empty;
        public string Manners { get; set; } = string.Empty;
        public string LanguageAssistance { get; set; } = string.Empty;
        public string ProperBehaviour { get; set; } = string.Empty;


        public static implicit operator CultureDTO(Culture data)
        {
            if (data == null)
                return null;

            return new CultureDTO
            {
                Id = data.Id,
                Name = data.Name,
                Description = data.Description,
                Country = data.Country,
                Religion = data.Religion,
                Goverment = data.Goverment,
                GeograpInformation = data.GeograpInformation,
                Manners = data.Manners,
                LanguageAssistance = data.LanguageAssistance,
                ProperBehaviour = data.ProperBehaviour
            };
        }
    }
}
