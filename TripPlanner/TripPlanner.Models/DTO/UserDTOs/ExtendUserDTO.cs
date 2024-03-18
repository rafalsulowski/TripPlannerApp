using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TripPlanner.Models.DTO.UserDTOs
{
    public class ExtendUserDTO : ObservableObject
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string FullAddress { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public bool IsActivated { get; set; }

        bool isFriend;
        public bool IsFriend
        {
            get => isFriend;
            set => SetProperty(ref isFriend, value);
        }
        public DateTime DateOfBirth { get; set; }
    }
}
