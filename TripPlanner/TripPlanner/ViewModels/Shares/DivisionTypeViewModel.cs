using CommunityToolkit.Maui.Core.Extensions;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using TripPlanner.Models.DTO.BillDTOs;
using TripPlanner.Models.DTO.TourDTOs;
using TripPlanner.Models.Models.BillModels;
using TripPlanner.Models.Models.UserModels;
using TripPlanner.Services;

namespace TripPlanner.ViewModels.Shares
{
    public partial class DivisionTypeViewModel : ObservableObject, IQueryAttributable
    {
        private readonly Configuration m_Configuration;
        private readonly ShareService m_ShareService;
        private readonly TourService m_TourService;
        private int TourId;

        [ObservableProperty]
        CreateBillDTO bill;

        [ObservableProperty]
        ObservableCollection<ExtendParticipantDTO> participants;
        
        [ObservableProperty]
        ObservableCollection<SplitBillView> splitBillViews;

        [ObservableProperty]
        SplitBillView currentItem;

        public DivisionTypeViewModel(Configuration configuration, ShareService shareService, TourService tourService)
        {
            m_TourService = tourService;
            m_ShareService = shareService;
            m_Configuration = configuration;

            Bill = new CreateBillDTO();
            Participants = new ObservableCollection<ExtendParticipantDTO>();
            SplitBillViews = new ObservableCollection<SplitBillView>();
        }

        public async void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            TourId = (int)query["passTourId"];
            Bill = (CreateBillDTO)query["passBill"];
            var splitBillView = (SplitBillView)query["passSplitBillView"];
            await LoadData();

            SplitBillViews.Clear();
            SplitBillView sbv1 = new SplitBillView
            {
                BillType = Models.Models.BillModels.BillType.Equally,
                Icon = "pen_sec.png",
                Title = "Równomiernie",
                Description = "Wybierz które osoby mają dzielić się wspólnie rachunkiem",
                IsCheckedAll = true,
                SplitFirstInfo = $"{(Bill.Value / Participants.Count):N2}zł / osobę",
                SplitSecondInfo = $"(wszyscy się składają)",
                Contributors = new ObservableCollection<ExtendBillContributorDTO>()
            };
            foreach (var participant in Participants)
            {
                sbv1.Contributors.Add(new ExtendBillContributorDTO
                {
                    UserId = participant.UserId,
                    Due = Bill.Value / Participants.Count,
                    Name = participant.FullName,
                    SplitValue = 0,
                    IsChecked = true
                });
            }
            SplitBillViews.Add(sbv1);

            SplitBillView sbv2 = new SplitBillView
            {
                BillType = Models.Models.BillModels.BillType.Unequally,
                Icon = "pen_sec.png",
                Title = "Nierównomiernie",
                Description = "Wprowadź dokładnie ile dane osoby są dłużne",
                SplitFirstInfo = $"{0:N2}zł z {Bill.Value}zł",
                SplitSecondInfo = $"pozostało {Bill.Value}zł",
                Contributors = new ObservableCollection<ExtendBillContributorDTO>()
            };
            foreach (var participant in Participants)
            {
                sbv2.Contributors.Add(new ExtendBillContributorDTO
                {
                    UserId = participant.UserId,
                    Due = 0,
                    SplitValue = 0,
                    Name = participant.FullName,
                    IsChecked = false
                });
            }
            SplitBillViews.Add(sbv2);

            SplitBillView sbv3 = new SplitBillView
            {
                BillType = Models.Models.BillModels.BillType.ByPercentages,
                Icon = "pen_sec.png",
                Title = "Procentowo",
                Description = "Wprowadź procentowy udział dla twojej sytuacji",
                IsCheckedAll = true,
                SplitFirstInfo = $"0% z 100%",
                SplitSecondInfo = $"pozostało 100%",
                Contributors = new ObservableCollection<ExtendBillContributorDTO>()
            };
            foreach (var participant in Participants)
            {
                sbv3.Contributors.Add(new ExtendBillContributorDTO
                {
                    UserId = participant.UserId,
                    Due = 0,
                    SplitValue = 0,
                    Name = participant.FullName,
                    IsChecked = false
                });
            }
            SplitBillViews.Add(sbv3);

            SplitBillView sbv4 = new SplitBillView
            {
                BillType = Models.Models.BillModels.BillType.ByShares,
                Icon = "pen_sec.png",
                Title = "Przez udział",
                Description = "Wprowadź wielkość udziału jaką wykorzystały dane osoby",
                IsCheckedAll = true,
                SplitFirstInfo = $"0 udziałów",
                SplitSecondInfo = "",
                Contributors = new ObservableCollection<ExtendBillContributorDTO>()
            };
            foreach (var participant in Participants)
            {
                sbv4.Contributors.Add(new ExtendBillContributorDTO
                {
                    UserId = participant.UserId,
                    Due = 0,
                    SplitValue = 0,
                    Name = participant.FullName,
                });
            }
            SplitBillViews.Add(sbv4);

            SplitBillView sbv5 = new SplitBillView
            {
                BillType = Models.Models.BillModels.BillType.ByAdjustment,
                Icon = "pen_sec.png",
                Title = "Przez dodanie wartości",
                Description = "Wprowadź kto musi dodatkowo zapłacić",
                IsCheckedAll = true,
                SplitFirstInfo = "",
                SplitSecondInfo = "",
                Contributors = new ObservableCollection<ExtendBillContributorDTO>()
            };
            foreach (var participant in Participants)
            {
                sbv5.Contributors.Add(new ExtendBillContributorDTO
                {
                    UserId = participant.UserId,
                    Due = Bill.Value / Participants.Count,
                    SplitValue = 0,
                    Name = participant.FullName,
                });
            }
            SplitBillViews.Add(sbv5);


            if (splitBillView != null)
            {
                switch (splitBillView.BillType)
                {
                    case BillType.Equally:
                        SplitBillViews[0] = splitBillView;
                        break;
                    case BillType.Unequally:
                        SplitBillViews[1] = splitBillView;
                        break;
                    case BillType.ByPercentages:
                        SplitBillViews[2] = splitBillView;
                        break;
                    case BillType.ByShares:
                        SplitBillViews[3] = splitBillView;
                        break;
                    case BillType.ByAdjustment:
                        SplitBillViews[4] = splitBillView;
                        break;
                    default:
                        break;
                }
            }
        }

        [RelayCommand]
        async Task GoBack()
        {
            var navigationParameter = new Dictionary<string, object>
            {
                { "passTourId",  TourId},
                { "passBill",  Bill},
                { "passSplitBillView",  CurrentItem},
                { "passSplitBillViewAccept",  false},
                { "passIsEditing",  false}
            };
            await Shell.Current.GoToAsync($"/Tour/Shares/CreateBill", navigationParameter);
        }

        [RelayCommand]
        async Task Accept()
        {
            switch (CurrentItem.BillType)
            {
                case BillType.Equally:
                    Bill.BillType = BillType.Equally;
                    if (SplitBillViews[0].Contributors.Count(u => u.IsChecked) == 0)
                    {
                        await Shell.Current.CurrentPage.DisplayAlert("Pomyłka", "Musisz wskazać co najmniej 1 osobę", "Ok");
                        return;
                    }
                    break;

                case BillType.Unequally:
                    Bill.BillType = BillType.Unequally;
                    if (SplitBillViews[1].Contributors.Sum(u => u.Due) != Bill.Value)
                    {
                        if (SplitBillViews[1].Contributors.Sum(u => u.Due) < Bill.Value)
                            await Shell.Current.CurrentPage.DisplayAlert("Błąd", $"Kwoty osób nie sumują się do wartości rachunku {Bill.Value}zł, brakuje {Bill.Value - SplitBillViews[1].Contributors.Sum(u => u.Due)}zł", "Ok");
                        else
                            await Shell.Current.CurrentPage.DisplayAlert("Błąd", $"Kwoty osób nie sumują się do wartości rachunku {Bill.Value}zł, nadmiarowe {SplitBillViews[1].Contributors.Sum(u => u.Due) - Bill.Value}zł", "Ok");
                        return;
                    }
                    break;

                case BillType.ByPercentages:
                    Bill.BillType = BillType.ByPercentages;
                    if (SplitBillViews[2].Contributors.Sum(u => u.SplitValue) != 100)
                    {
                        if (SplitBillViews[2].Contributors.Sum(u => u.SplitValue) < 100)
                            await Shell.Current.CurrentPage.DisplayAlert("Błąd", $"Procenty osób nie sumują się do 100%, brakuje {100 - SplitBillViews[2].Contributors.Sum(u => u.SplitValue)}%", "Ok");
                        else
                            await Shell.Current.CurrentPage.DisplayAlert("Błąd", $"Procenty osób nie sumują się do 100%, nadmiarowe {SplitBillViews[2].Contributors.Sum(u => u.SplitValue) - 100}%", "Ok");
                        return;
                    }
                    break;

                case BillType.ByShares:
                    Bill.BillType = BillType.ByShares;
                    if (SplitBillViews[3].Contributors.Sum(u => u.SplitValue) == 0)
                    {
                        await Shell.Current.CurrentPage.DisplayAlert("Błąd", $"Musisz wprowadzić co najmniej 1 udział", "Ok");
                        return;
                    }
                    break;

                case BillType.ByAdjustment:
                    Bill.BillType = BillType.ByAdjustment;
                    break;

                default:
                    break;
            }

            var navigationParameter = new Dictionary<string, object>
            {
                { "passTourId",  TourId},
                { "passBill",  Bill},
                { "passSplitBillView",  CurrentItem},
                { "passSplitBillViewAccept",  true},
                { "passIsEditing",  false}
            };
            await Shell.Current.GoToAsync($"/Tour/Shares/CreateBill", navigationParameter);
        }
        
        private async Task LoadData()
        {
            var res = await m_TourService.GetTourExtendParticipant(TourId);
            if(res.Any())
            {
                Participants = res.ToObservableCollection();
            }
        }

        #region Equally
        [RelayCommand]
        async Task CheckAll()
        {
            if(!SplitBillViews[0].IsCheckedAll)
            {
                SplitBillViews[0].IsCheckedAll = true;
                decimal val = Bill.Value / SplitBillViews[0].Contributors.Count;
                SplitBillViews[0].SplitFirstInfo = $"{val:N2}zł / osobę";
                SplitBillViews[0].SplitSecondInfo = $"(wszyscy się składają)";
                for (int i = 0; i < SplitBillViews[0].Contributors.Count; i++)
                {
                    SplitBillViews[0].Contributors[i].Due = Bill.Value / SplitBillViews[0].Contributors.Count;
                    SplitBillViews[0].Contributors[i].IsChecked = true;
                }

                SplitBillViews[0].SplitSecondInfo = $"(wszyscy się składają)";
            }
            else
            {
                SplitBillViews[0].IsCheckedAll = false;
                SplitBillViews[0].SplitFirstInfo = $"0zł / osobę";
                SplitBillViews[0].SplitSecondInfo = $"(nikt się nie składa)";
                for (int i = 0; i < SplitBillViews[0].Contributors.Count; i++)
                {
                    SplitBillViews[0].Contributors[i].Due = 0;
                    SplitBillViews[0].Contributors[i].IsChecked = false;
                }

                SplitBillViews[0].SplitSecondInfo = $"(nikt się nie składa)";
            }
        }

        [RelayCommand]
        async Task Check(ExtendBillContributorDTO contributor)
        {
            if (contributor == null)
                return;

            int index = SplitBillViews[0].Contributors.IndexOf(contributor);
            SplitBillViews[0].Contributors[index].IsChecked = !SplitBillViews[0].Contributors[index].IsChecked;

            //obliczenie nowego rozkladu pieniedzy
            int personCount = SplitBillViews[0].Contributors.Count(u => u.IsChecked == true);
            if(personCount != 0)
                for (int i = 0; i < SplitBillViews[0].Contributors.Count; i++)
                {
                    if (SplitBillViews[0].Contributors[i].IsChecked)
                        SplitBillViews[0].Contributors[i].Due = Bill.Value / personCount;
                    else
                        SplitBillViews[0].Contributors[i].Due = 0;
                }
            else
                for (int i = 0; i < SplitBillViews[0].Contributors.Count; i++)
                    SplitBillViews[0].Contributors[i].Due = 0; //safe after dividing by 0


            //aktualziacja napisów
            if (personCount != 0)
            {
                decimal val = Bill.Value / personCount;
                SplitBillViews[0].SplitFirstInfo = $"{val:N2}zł / osobę";
                if (SplitBillViews[0].Contributors.Count == personCount)
                {
                    SplitBillViews[0].IsCheckedAll = true;
                    SplitBillViews[0].SplitSecondInfo = $"(wszyscy się składają)";
                }
                else
                {
                    SplitBillViews[0].IsCheckedAll = false;
                    SplitBillViews[0].SplitSecondInfo = $"({personCount} składających się)";
                }
            }
            else
            {
                SplitBillViews[0].IsCheckedAll = false;
                SplitBillViews[0].SplitFirstInfo = $"0zł / osobę";
                SplitBillViews[0].SplitSecondInfo = $"(nikt się nie składa)";
            }
        }
        #endregion

        #region UnEqually
        [RelayCommand]
        async Task UnEquallyDueChanged(ExtendBillContributorDTO contributor)
        {
            if (contributor == null)
                return;

            decimal sum = SplitBillViews[1].Contributors.Sum(u => u.Due);
            SplitBillViews[1].SplitFirstInfo = $"{sum}zł z {Bill.Value}zł";
            decimal sub = Bill.Value - sum;
            if (sub < 0)
                SplitBillViews[1].SplitSecondInfo = $"nadmiarowe {-sub}zł";
            else
                SplitBillViews[1].SplitSecondInfo = $"pozostało {sub}zł";
        }
        #endregion

        #region ByPercentages
        [RelayCommand]
        async Task ByPercentagesDueChanged(ExtendBillContributorDTO contributor)
        {
            if (contributor == null)
                return;

            //obliczenie wartości
            decimal val = Bill.Value * contributor.SplitValue / 100;
            int index = SplitBillViews[2].Contributors.IndexOf(contributor);
            SplitBillViews[2].Contributors[index].Due = val;

            //Modyfikacja napisów
            int sum = (int)SplitBillViews[2].Contributors.Sum(u => decimal.Floor(Math.Abs(u.SplitValue)));
            SplitBillViews[2].SplitFirstInfo = $"{sum}% z 100%";
            int sub = 100 - sum;
            if (sub < 0)
                SplitBillViews[2].SplitSecondInfo = $"nadmiarowe {-sub}%";
            else
                SplitBillViews[2].SplitSecondInfo = $"pozostało {sub}%";
        }
        #endregion

        #region ByShares
        [RelayCommand]
        async Task BySharesDueChanged(ExtendBillContributorDTO contributor)
        {
            if (contributor == null)
                return;

            //obliczenie wartości
            decimal sumShares = SplitBillViews[3].Contributors.Sum(u => Math.Abs(u.SplitValue));
            decimal val = Bill.Value * contributor.SplitValue / sumShares;
            int index = SplitBillViews[3].Contributors.IndexOf(contributor);
            SplitBillViews[3].Contributors[index].Due = val;

            //Modyfikacja napisów
            int sum = (int)SplitBillViews[3].Contributors.Sum(u => Math.Abs(u.Due));
            SplitBillViews[3].SplitFirstInfo = $"{sum} udziałów";
        }
        #endregion

        #region ByAdjustment
        [RelayCommand]
        async Task ByAdjustmentDueChanged(ExtendBillContributorDTO contributor)
        {
            if (contributor == null)
                return;

            if(contributor.SplitValue > Bill.Value)
            {
                await Shell.Current.CurrentPage.DisplayAlert("Błąd", "Podałeś wartość która przekracza wysokość rachunku", "Popraw");
                contributor.SplitValue = 0;
            }

            //obliczenie wartości
            int size = SplitBillViews[4].Contributors.Count;
            decimal sumAdjustment = SplitBillViews[4].Contributors.Sum(u => u.SplitValue);
            for(int i = 0; i < size; i++)
            {
                SplitBillViews[4].Contributors[i].Due = (Bill.Value - sumAdjustment) / size;
                SplitBillViews[4].Contributors[i].Due += SplitBillViews[4].Contributors[i].SplitValue;
            }
        }
        #endregion
    }
}
