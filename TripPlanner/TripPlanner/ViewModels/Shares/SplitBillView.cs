using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using TripPlanner.Models.DTO.BillDTOs;
using TripPlanner.Models.Models.BillModels;

namespace TripPlanner.ViewModels.Shares
{
    public partial class SplitBillView : ObservableObject
    {
        private BillType billType;
        public BillType BillType
        {
            get => billType;
            set => SetProperty(ref billType, value);
        }

        public string icon;
        public string Icon
        {
            get => icon;
            set => SetProperty(ref icon, value);
        }

        public string title;
        public string Title
        {
            get => title;
            set => SetProperty(ref title, value);
        }

        public string description;
        public string Description
        {
            get => description;
            set => SetProperty(ref description, value);
        }

        public string splitFirstInfo;
        public string SplitFirstInfo
        {
            get => splitFirstInfo;
            set => SetProperty(ref splitFirstInfo, value);
        }

        public string splitSecondInfo;
        public string SplitSecondInfo
        {
            get => splitSecondInfo;
            set => SetProperty(ref splitSecondInfo, value);
        }

        public bool isCheckedAll;
        public bool IsCheckedAll
        {
            get => isCheckedAll;
            set => SetProperty(ref isCheckedAll, value);
        }

        public ObservableCollection<ExtendBillContributorDTO> contributors;
        public ObservableCollection<ExtendBillContributorDTO> Contributors
        {
            get => contributors;
            set => SetProperty(ref contributors, value);
        }
    }
}
