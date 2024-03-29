﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TripPlanner.Models.DTO.TourDTOs;
using TripPlanner.Models.DTO.UserDTOs;

namespace TripPlanner.DataTemplates
{
    public class IsFriendDataTemplateSelector : DataTemplateSelector
    {
        public DataTemplate Friend { get; set; }
        public DataTemplate IsNotFriend { get; set; }

        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            return ((ExtendUserDTO)item).IsFriend ? Friend : IsNotFriend;
        }
    }
}
