using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TripPlanner.Models.DTO.UserDTOs;
using TripPlanner.Models.Models.TourModels;

namespace TripPlanner.Models.Models.UserModels
{
    public class Friend
    {
        public User Friend1 { get; set; } = null!;
        public int Friend1Id { get; set; }
        public User Friend2 { get; set; } = null!;
        public int Friend2Id { get; set; }


        public static implicit operator FriendDTO(Friend data)
        {
            if (data == null)
                return null;

            return new FriendDTO
            {
                Friend1Id = data.Friend1Id,
                Friend2Id = data.Friend2Id
            };
        }
    }
}
