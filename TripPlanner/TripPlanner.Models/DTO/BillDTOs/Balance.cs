using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using TripPlanner.Models.Models.TourModels;

namespace TripPlanner.Models.DTO.BillDTOs
{
    public class Balance
    {
        public decimal TotalBalance { get; set; }
        public List<UserBalance> UserBalances { get; set; } = new List<UserBalance>();
    }

    public class UserBalance : ObservableObject
    {
        public int UserId { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Saldo { get; set; } // wartosc na minusie oznacza ze ktos porzyczył pieniądze, na plusie że ktoś ma do oddania
        public List<OtherUser> BalanceWithOtherUsers { get; set; } = new List<OtherUser>(); //uczestnicy ktorzy wisią temu uczestnikowi lub na odwrót


        public bool isExpand;
        public bool IsExpand
        {
            get => isExpand;
            set => SetProperty(ref isExpand, value);
        }
    }

    public class OtherUser
    {
        public int UserBalanceId { get; set; }
        public int UserId { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Saldo { get; set; } // wartosc na minusie oznacza ze ktos porzyczył pieniądze, na plusie że ktoś ma do oddania
    }
}
