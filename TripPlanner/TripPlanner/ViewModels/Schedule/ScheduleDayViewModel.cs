using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Core.Extensions;
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using TripPlanner.Models.DTO.ScheduleDTOs;
using TripPlanner.Models.DTO.TourDTOs;
using TripPlanner.Services;
using TripPlanner.Views.ScheduleViews;
using CommunityToolkit.Maui.Storage;
using System.Threading;

namespace TripPlanner.ViewModels.Schedule
{
    public partial class ScheduleDayViewModel : ObservableObject, IQueryAttributable
    {
        private readonly Configuration m_Configuration;
        private readonly ScheduleService m_ScheduleService;
        private int TourId;
        private int ScheduleDayId;
        private List<int> Days;
        private bool IsFromCalendarPage;

        [ObservableProperty]
        ScheduleDayDTO schedule;

        [ObservableProperty]
        ObservableCollection<ScheduleEventDTO> events;

        [ObservableProperty]
        bool emptyLabel;

        public ScheduleDayViewModel(Configuration configuration, ScheduleService scheduleService, TourService tourService, IFileSaver fileSaver)
        {
            m_Configuration = configuration;
            m_ScheduleService = scheduleService;
            IsFromCalendarPage = false;
            Events = new ObservableCollection<ScheduleEventDTO>();
        }

        public async void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            TourId = (int)query["passTourId"];
            ScheduleDayId = (int)query["passScheduleDayId"];
            Days = (List<int>)query["passListDays"];
            IsFromCalendarPage = (bool)query["IsFromCalendarPage"];
            await LoadData();
        }

        [RelayCommand]
        async Task GoBack()
        {
            var navigationParameter = new Dictionary<string, object>
            {
                { "passTourId",  TourId}
            };
            if(IsFromCalendarPage)
                await Shell.Current.GoToAsync($"/Tour/ScheduleCalendar", navigationParameter);
            else
                await Shell.Current.GoToAsync($"/Tour/ScheduleList", navigationParameter);
        }

        [RelayCommand]
        async Task GoPrevious()
        {
            int index = Days.IndexOf(Schedule.Id);
            if (index < 1)
                return;

            ScheduleDayId = Days[index - 1];
            await LoadData();
        }

        [RelayCommand]
        async Task GoNext()
        {
            int index = Days.IndexOf(Schedule.Id);
            if (index == Days.Count - 1)
                return;

            ScheduleDayId = Days[index + 1];
            await LoadData();
        }

        [RelayCommand]
        async Task DeleteEvent(ScheduleEventDTO eventDto)
        {
            var res = await Shell.Current.CurrentPage.DisplayAlert("Uwaga","Czy na pewno chcesz usunąć punkt harmonogramu?", "Tak", "Nie");

            if(res)
            {
                var response = await m_ScheduleService.DeleteScheduleEvent(eventDto.ScheduleDayId, eventDto.Id, m_Configuration.User.Id);

                if(response.Success)
                {
                    var confirmCopyToast = Toast.Make("Usunięto punkt harmonogramu", ToastDuration.Short, 14);
                    await confirmCopyToast.Show();
                    await LoadData();
                }
                else
                    await Shell.Current.CurrentPage.DisplayAlert("Uwaga", $"Nie udało się usunąć punktu z harmonogramu, ponieważ: {response.Message}", "Ok");
            }
        }

        [RelayCommand]
        async Task AddEvent()
        {
            var navigationParameter = new Dictionary<string, object>
            {
                { "passTourId",  TourId},
                { "passSchedule",  Schedule},
                { "passEvent",  null},
                { "passListDays",  Days},
                { "IsFromCalendarPage", IsFromCalendarPage }
            };
            await Shell.Current.GoToAsync($"/Tour/ScheduleDay/Event", navigationParameter);
        }

        [RelayCommand]
        async Task UpdateEvent(ScheduleEventDTO eventDto)
        {
            var navigationParameter = new Dictionary<string, object>
            {
                { "passTourId",  TourId},
                { "passSchedule",  Schedule},
                { "passEvent",  eventDto},
                { "passListDays",  Days},
                { "IsFromCalendarPage", IsFromCalendarPage }
            };
            await Shell.Current.GoToAsync($"/Tour/ScheduleDay/Event", navigationParameter);
        }
        
        private async Task LoadData()
        {
            var result = await m_ScheduleService.GetScheduleDay(ScheduleDayId);
            Schedule = result;
            Events = (from p in Schedule.Events
                     orderby p.StartTime
                     select p).ToObservableCollection();

            if (Events.Count == 0)
                EmptyLabel = true;
            else
                EmptyLabel = false;
        }
    }
}
