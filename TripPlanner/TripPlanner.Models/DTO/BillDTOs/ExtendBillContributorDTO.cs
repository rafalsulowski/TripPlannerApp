using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TripPlanner.Models.Models.BillModels;

namespace TripPlanner.Models.DTO.BillDTOs
{
    public class ExtendBillContributorDTO : ObservableObject
    {
        public bool isChecked; //dla Equally
        public bool IsChecked
        {
            get => isChecked;
            set => SetProperty(ref isChecked, value);
        }

        public decimal splitValue; //dla podzialu procentowego, przez udzial, przez dodanie
        public decimal SplitValue
        {
            get => splitValue;
            set => SetProperty(ref splitValue, value);
        }

        public int userId;
        public int UserId
        {
            get => userId;
            set => SetProperty(ref userId, value);
        }

        public string name;
        public string Name
        {
            get => name;
            set => SetProperty(ref name, value);
        }

        public decimal due;
        public decimal Due
        {
            get => due;
            set => SetProperty(ref due, value);
        }
    }
}
