using TripPlanner.Models.Models.CultureModels;

namespace TripPlanner.Models.DTO.CultureDTOs
{
    public class CreateCultureDTO
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string Religion { get; set; } = string.Empty;
        public string Goverment { get; set; } = string.Empty;
        public string GeograpInformation { get; set; } = string.Empty;
        public string Manners { get; set; } = string.Empty;
        public string LanguageAssistance { get; set; } = string.Empty;
        public string ProperBehaviour { get; set; } = string.Empty;


        public static implicit operator Culture(CreateCultureDTO data)
        {
            if (data == null)
                return null;

            return new Culture
            {
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
