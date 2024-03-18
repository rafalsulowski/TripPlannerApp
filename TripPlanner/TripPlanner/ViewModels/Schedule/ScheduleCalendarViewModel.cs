using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Core.Extensions;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using TripPlanner.Models.DTO.ScheduleDTOs;
using TripPlanner.Services;

namespace TripPlanner.ViewModels.Schedule
{
    public partial class ScheduleCalendarViewModel : ObservableObject, IQueryAttributable
    {
        private readonly Configuration m_Configuration;
        private readonly ScheduleService m_ScheduleService;
        private int TourId;

        [ObservableProperty]
        ObservableCollection<ScheduleDayDTO> schedules;

        public ScheduleCalendarViewModel(Configuration configuration, ScheduleService scheduleService)
        {
            m_Configuration = configuration;
            m_ScheduleService = scheduleService;
            Schedules = new ObservableCollection<ScheduleDayDTO>();
        }

        public async void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            TourId = (int)query["passTourId"];
            await LoadData();
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
        async Task GoDay(ScheduleDayDTO day)
        {
            List<int> days = new List<int>();
            foreach (var dayy in Schedules)
            {
                days.Add(dayy.Id);
            }

            var navigationParameter = new Dictionary<string, object>
            {
                { "passTourId",  TourId},
                { "passScheduleDayId",  day.Id},
                { "passListDays",  days},
                { "IsFromCalendarPage", true }
            };
            await Shell.Current.GoToAsync($"/ScheduleDay", navigationParameter);
        }

        [RelayCommand]
        async Task GoCalendar()
        {
            var confirmCopyToast = Toast.Make("Funkcjonalność niezaimplementowana!", ToastDuration.Long, 14);
            await confirmCopyToast.Show();
        }

        [RelayCommand]
        async Task Export()
        {
            var confirmCopyToast = Toast.Make("Funkcjonalność niezaimplementowana!", ToastDuration.Long, 14);
            await confirmCopyToast.Show();
        }

        private async Task LoadData()
        {
            var result = await m_ScheduleService.GetSchedule(TourId);
            Schedules = result.ToObservableCollection();
        }
    }
}
