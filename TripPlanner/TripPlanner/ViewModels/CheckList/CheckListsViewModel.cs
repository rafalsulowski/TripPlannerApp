using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MvvmHelpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.Metrics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TripPlanner.Models.DTO.BillDTOs;
using TripPlanner.Models.DTO.CheckListDTOs;
using TripPlanner.Models.DTO.MessageDTOs.QuestionnaireDTOs;
using TripPlanner.Models.Models.BillModels;
using TripPlanner.Models.Models.CheckListModels;
using TripPlanner.Services;

namespace TripPlanner.ViewModels.CheckList
{
    public enum ViewType
    {
        CheckLists,
        Questionnaires
    }

    public partial class CheckListView : CommunityToolkit.Mvvm.ComponentModel.ObservableObject
    {
        private ViewType viewType;
        public ViewType ViewType
        {
            get => viewType;
            set => SetProperty(ref viewType, value);
        }

        private string name;
        public string Name
        {
            get => name;
            set => SetProperty(ref name, value);
        }

        private List<CheckListGroup> groups;
        public List<CheckListGroup> Groups
        {
            get => groups;
            set => SetProperty(ref groups, value);
        }

        private List<QuestionnaireDTO> questionnaires;
        public List<QuestionnaireDTO> Questionnaires
        {
            get => questionnaires;
            set => SetProperty(ref questionnaires, value);
        }
    }

    public class CheckListGroup : ObservableRangeCollection<CheckListDTO>, INotifyPropertyChanged
    {
        public string Name { get; set; }

        private string groupIcon = "caret_up_dt.png";
        public string GroupIcon
        {
            get => groupIcon;
            set => SetProperty(ref groupIcon, value);
        }

        public CheckListGroup(string name, List<CheckListDTO> checkLists) : base(checkLists)
        {
            Name = name;
        }

        protected bool SetProperty<T>(ref T backingStore, T value,
        [CallerMemberName] string propertyName = "",
        Action onChanged = null)
        {
            if (EqualityComparer<T>.Default.Equals(backingStore, value))
                return false;

            backingStore = value;
            onChanged?.Invoke();
            OnPropertyChanged(propertyName);
            return true;
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            var changed = PropertyChanged;
            if (changed == null)
                return;

            changed.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }

    public partial class CheckListsViewModel : CommunityToolkit.Mvvm.ComponentModel.ObservableObject, IQueryAttributable
    {
        private readonly ChatService m_ChatService;
        private readonly Configuration m_Configuration;
        private readonly CheckListService m_CheckListService;
        private List<QuestionnaireDTO> Questionnaires;
        private List<CheckListDTO> CheckLists;
        private List<CheckListGroup> Groups;
        private int TourId;

        [ObservableProperty]
        ObservableCollection<CheckListView> checkListViews;

        [ObservableProperty]
        CheckListView currentItem;

        public CheckListsViewModel(Configuration configuration, CheckListService checkListService, ChatService chatService)
        {
            m_Configuration = configuration;
            m_CheckListService = checkListService;
            m_ChatService = chatService;

            CheckListViews = new ObservableCollection<CheckListView>();
            Groups = new List<CheckListGroup>();
            Questionnaires = new List<QuestionnaireDTO>();
        }

        public async void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            TourId = (int)query["passTourId"];
            await LoadData();

            CheckListView sbv1 = new CheckListView
            {
                ViewType = ViewType.CheckLists,
                Name = "CheckListy",
                Groups = Groups,
                Questionnaires = new List<QuestionnaireDTO>()
            };
            CheckListViews.Add(sbv1);

            CheckListView sbv2 = new CheckListView
            {
                ViewType = ViewType.Questionnaires,
                Name = "Ankiety",
                Groups = new List<CheckListGroup>(),
                Questionnaires = Questionnaires
            };
            CheckListViews.Add(sbv2);
            CurrentItem = CheckListViews[0];
        }


        [RelayCommand]
        async Task GoBack()
        {
            var navigationParameter = new Dictionary<string, object>
            {
                { "passTourId",  TourId}
            };
            await Shell.Current.GoToAsync($"/Tour", navigationParameter);
        }

        [RelayCommand]
        async Task GoQuestionnaire(QuestionnaireDTO questionnaire)
        {
            var navigationParameter = new Dictionary<string, object>
            {
                { "passTourId",  TourId},
                { "passQuestionnaireId", questionnaire.Id }
            };
            await Shell.Current.GoToAsync($"/Tour/CheckLists/Questionnaire", navigationParameter);
        }

        [RelayCommand]
        async Task GoCheckList(CheckListDTO checkList)
        {
            var navigationParameter = new Dictionary<string, object>
            {
                { "passTourId",  TourId},
                { "passCheckListId",  checkList.Id}
            };
            await Shell.Current.GoToAsync($"/Tour/CheckLists/CheckList", navigationParameter);
        }


        [RelayCommand]
        async Task AddCheckList(CheckListGroup group)
        {
            bool tmp = false;
            if (group.Name == "Publiczne checklisty")
                tmp = true;
            
            var navigationParameter = new Dictionary<string, object>
            {
                { "passTourId",  TourId},
                { "IsPublic",  tmp}
            };
            await Shell.Current.GoToAsync($"/Tour/CheckLists/CreateCheckList", navigationParameter);
        }

        [RelayCommand]
        async Task AddQuestionnaire()
        {
            var navigationParameter = new Dictionary<string, object>
            {
                { "passTourId",  TourId},
                { "IsFromChat",  false},
            };
            await Shell.Current.GoToAsync($"/Tour/Chat/CreateQuestionnaire", navigationParameter);
        }

        public ICommand AddOrRemoveGroupDataCommand => new Command<CheckListGroup>((item) =>
        {
            if (item.GroupIcon == "caret_up_dt.png")
            {
                item.Clear();
                item.GroupIcon = "caret_down_dt.png";
            }
            else
            {
                var recordsTobeAdded = CheckLists.Where(f => item.Name == "Publiczne checkListy" ? f.IsPublic == true : f.IsPublic == false).ToList();
                item.AddRange(recordsTobeAdded);
                item.GroupIcon = "caret_up_dt.png";
            }
        });

        private async Task LoadData()
        {
            Groups.Clear();
            Groups.Add(new CheckListGroup("Publiczne checklisty", new List<CheckListDTO>())); //zał. index 0 === publiczne
            Groups.Add(new CheckListGroup("Moje prywatne checklisty", new List<CheckListDTO>()));  //zał. index 1 === prywatne

            var result = await m_CheckListService.GetCheckListFromTour(TourId);
            if (result != null)
            {
                CheckLists = result;
                foreach (var item in result)
                {
                    if (item.IsPublic)
                        Groups[0].Add(item); //dodaj do publicznej
                    else
                    {
                        if(item.UserId == m_Configuration.User.Id)
                            Groups[1].Add(item);
                    }
                }
            }

            Questionnaires.Clear();
            var result2 = await m_ChatService.GetQuestionnairesFromTour(TourId);
            if (result2 != null)
                Questionnaires = result2;
        }
    }
}
