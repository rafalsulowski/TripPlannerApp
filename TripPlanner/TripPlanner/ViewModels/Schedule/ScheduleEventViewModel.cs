
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using TripPlanner.Models.DTO.ScheduleDTOs;
using TripPlanner.Models.Models.ScheduleModels;
using TripPlanner.Services;

namespace TripPlanner.ViewModels.Schedule
{
    public partial class ScheduleEventViewModel : ObservableObject, IQueryAttributable
    {
        private readonly Configuration m_Configuration;
        private readonly ScheduleService m_ScheduleService;
        private int TourId;
        private List<int> Days;
        private bool IsFromCalendarPage;

        [ObservableProperty]
        ScheduleEventDTO eventDto;

        [ObservableProperty]
        ScheduleDayDTO schedule;

        [ObservableProperty]
        string headerLabel;

        [ObservableProperty]
        TimeSpan startTime;

        [ObservableProperty]
        TimeSpan stopTime;

        [ObservableProperty]
        string name;

        [ObservableProperty]
        bool isStopTimeActive;

        [ObservableProperty]
        bool isEditing;

        public ScheduleEventViewModel(Configuration configuration, ScheduleService scheduleService)
        {
            m_Configuration = configuration;
            m_ScheduleService = scheduleService;
            IsStopTimeActive = false;
            IsEditing = false;
            StartTime = new TimeSpan(12, 0, 0);
            StopTime = new TimeSpan(12, 0, 0);
            HeaderLabel = "";
        }

        public async void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            TourId = (int)query["passTourId"];
            Schedule = (ScheduleDayDTO)query["passSchedule"];
            EventDto = (ScheduleEventDTO)query["passEvent"];
            Days = (List<int>)query["passListDays"];
            IsFromCalendarPage = (bool)query["IsFromCalendarPage"];  

            HeaderLabel = $"Nowy punkt czasowy {Schedule.Date:dd.MM.yyyy} {m_Configuration.GetLongNameOfDayWeek(Schedule.Date)}";

            if (EventDto != null)
            {
                HeaderLabel = $"Edytujesz punkt czasowy {Schedule.Date:dd.MM.yyyy} {m_Configuration.GetLongNameOfDayWeek(Schedule.Date)}";
                IsEditing = true;
                if (EventDto.StartTime != EventDto.StopTime)
                    IsStopTimeActive = true;

                Name = EventDto.Name;
                StartTime = new TimeSpan(EventDto.StartTime.Hour, EventDto.StartTime.Minute, 0);
                StopTime = new TimeSpan(EventDto.StopTime.Hour, EventDto.StopTime.Minute, 0);

                int hour = (StopTime - StartTime).Hours;
                int Minute = (StopTime - StartTime).Minutes;
            }
        }

        [RelayCommand]
        async Task GoBack()
        {
            var navigationParameter = new Dictionary<string, object>
            {
                { "passTourId",  TourId},
                { "passScheduleDayId",  Schedule.Id},
                { "passListDays",  Days},
                { "IsFromCalendarPage",  IsFromCalendarPage},
            };
            await Shell.Current.GoToAsync($"/Tour/ScheduleDay", navigationParameter);
        }

        [RelayCommand]
        async Task ExpandStopTime()
        {
            IsStopTimeActive = !IsStopTimeActive;
        }


        [RelayCommand]
        async Task Submit()
        {
            if (string.IsNullOrEmpty(Name))
            {
                await Shell.Current.CurrentPage.DisplayAlert("Błąd", "Nazwa punktu harmoongramu nie może być pusta", "Popraw");
                return;
            }

            if (IsStopTimeActive)
            {
                if (StopTime < StartTime)
                {
                    await Shell.Current.CurrentPage.DisplayAlert("Błąd", "Data zakończenia jest ustawiona przed datą zakończenia", "Popraw");
                    return;
                }
            }
            else
                StopTime = StartTime;


            foreach (var eve in Schedule.Events)
            {
                if (EventDto != null && EventDto.Id == eve.Id)
                    continue;

                int eveMinuteStartTime = eve.StartTime.Hour * 60 + eve.StartTime.Minute;
                int eveMinuteStopTime = eve.StopTime.Hour * 60 + eve.StopTime.Minute;
                int userMinuteStartTime = StartTime.Hours * 60 + StartTime.Minutes;
                int userMinuteStopTime = StopTime.Hours * 60 + StopTime.Minutes;

                if (!((userMinuteStartTime < eveMinuteStartTime && userMinuteStopTime < eveMinuteStartTime)
                    || (userMinuteStartTime > eveMinuteStopTime && userMinuteStopTime > eveMinuteStopTime)))
                {
                    string duration = "";
                    if (eveMinuteStartTime == eveMinuteStopTime)
                        duration = eve.StartTime.ToString("HH:mm");
                    else
                        duration = $"{eve.StartTime:HH:mm} - {eve.StopTime:HH:mm}";

                    var rews = await Shell.Current.CurrentPage.DisplayAlert("Uwaga", $"Wybrany czas wydarzenia pokrywaja się z innym wydarzeniem w harmonogramie: \"{eve.Name}\" odbywajacym się w czasie: {duration}", "Popraw", "Ignoruj");
                    if (rews)
                        return;
                    else
                        break;
                }
            }


            if (!IsEditing)
            {
                CreateScheduleEventDTO createScheduleEventDTO = new CreateScheduleEventDTO
                {
                    ScheduleDayId = Schedule.Id,
                    Name = Name,
                    StartTime = new DateTime(Schedule.Date.Year, Schedule.Date.Month, Schedule.Date.Day, StartTime.Hours, StartTime.Minutes, 0),
                    StopTime = new DateTime(Schedule.Date.Year, Schedule.Date.Month, Schedule.Date.Day, StartTime.Hours, StartTime.Minutes, 0),
                };

                if (IsStopTimeActive)
                {
                    createScheduleEventDTO.StopTime = new DateTime(Schedule.Date.Year, Schedule.Date.Month, Schedule.Date.Day, StopTime.Hours, StopTime.Minutes, 0);
                }

                var response = await m_ScheduleService.CreateScheduleEvent(createScheduleEventDTO);
                if (response.Success)
                {
                    var confirmCopyToast = Toast.Make("Dodano nowy punkt do harmonogramu", ToastDuration.Short, 14);
                    await confirmCopyToast.Show();
                    var navigationParameter = new Dictionary<string, object>
                    {
                        { "passTourId",  TourId},
                        { "passScheduleDayId",  Schedule.Id},
                        { "passListDays",  Days},
                        { "IsFromCalendarPage",  IsFromCalendarPage},
                    };
                    await Shell.Current.GoToAsync($"/Tour/ScheduleDay", navigationParameter);
                }
                else
                    await Shell.Current.CurrentPage.DisplayAlert("Uwaga", response.Message, "Ok");
            }
            else
            {
                EditScheduleEventDTO editScheduleEventDTO = new EditScheduleEventDTO
                {
                    Name = Name,
                    StartTime = new DateTime(Schedule.Date.Year, Schedule.Date.Month, Schedule.Date.Day, StartTime.Hours, StartTime.Minutes, 0),
                    StopTime = new DateTime(Schedule.Date.Year, Schedule.Date.Month, Schedule.Date.Day, StartTime.Hours, StartTime.Minutes, 0),
                };

                if (IsStopTimeActive)
                {
                    editScheduleEventDTO.StopTime = new DateTime(Schedule.Date.Year, Schedule.Date.Month, Schedule.Date.Day, StopTime.Hours, StopTime.Minutes, 0);
                }

                var response = await m_ScheduleService.UpdateScheduleEvent(Schedule.Id, EventDto.Id, m_Configuration.User.Id, editScheduleEventDTO);
                if (response.Success)
                {
                    var confirmCopyToast = Toast.Make("Zmodyfikowano punkt harmonogramu", ToastDuration.Short, 14);
                    await confirmCopyToast.Show();
                    var navigationParameter = new Dictionary<string, object>
                    {
                        { "passTourId",  TourId},
                        { "passScheduleDayId",  Schedule.Id},
                        { "passListDays",  Days},
                        { "IsFromCalendarPage",  IsFromCalendarPage},
                    };
                    await Shell.Current.GoToAsync($"/Tour/ScheduleDay", navigationParameter);
                }
                else
                    await Shell.Current.CurrentPage.DisplayAlert("Uwaga", response.Message, "Ok");
            }
        }
    }
}
