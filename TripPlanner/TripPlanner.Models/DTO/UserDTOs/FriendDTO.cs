using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TripPlanner.Models.Models.UserModels;

namespace TripPlanner.Models.DTO.UserDTOs
{
    public class FriendDTO
    {
        public int Friend1Id { get; set; }
        public int Friend2Id { get; set; }


        public static implicit operator Friend(FriendDTO data)
        {
            if (data == null)
                return null;

            return new Friend
            {
                Friend1Id = data.Friend1Id,
                Friend2Id = data.Friend2Id
            };
        }
    }
}
