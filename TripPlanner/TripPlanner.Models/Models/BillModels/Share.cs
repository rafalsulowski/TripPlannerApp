using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TripPlanner.Models.DTO.BillDTOs;
using TripPlanner.Models.DTO.MessageDTOs.QuestionnaireDTOs;
using TripPlanner.Models.DTO.MessageDTOs;
using TripPlanner.Models.Models.MessageModels.QuestionnaireModels;
using TripPlanner.Models.Models.MessageModels;
using TripPlanner.Models.Models.TourModels;
using TripPlanner.Models.Models.UserModels;

namespace TripPlanner.Models.Models.BillModels
{
    public abstract class Share
    {
        public int Id { get; set; }

        public int TourId { get; set; }
        public Tour Tour { get; set; } = null!;
        public int CreatorId { get; set; }
        public User Creator { get; set; } = null!;

        public DateTime CreatedDate { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public decimal Value { get; set; }
        public string ImageFilePath { get; set; } = string.Empty;


        public static implicit operator ShareDTO(Share data)
        {
            if (data == null)
                return null;

            if (data is Bill)
            {
                return (BillDTO)data;
            }
            else if (data is Transfer)
            {
                return (TransferDTO)data;
            }
            else
                return null;
        }
    }
}
