using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Views;
using TripPlanner.Models.DTO.ScheduleDTOs;
using TripPlanner.Services;

namespace TripPlanner.Views.ScheduleViews;

public partial class EventPopups : Popup
{
	private ScheduleService m_ScheduleService;
	private Configuration m_Configuration;
	private ScheduleEventDTO m_Event;
	private ScheduleDayDTO m_Schedule;
	private bool IsEditing;

	public EventPopups(ScheduleService scheduleService, Configuration config, ScheduleDayDTO schedule, ScheduleEventDTO editingEvent)
	{
		InitializeComponent();
        m_ScheduleService = scheduleService;
        m_Configuration = config;
		m_Event = editingEvent;
        m_Schedule = schedule;
		IsEditing = false;
        CheckBoxStopTime.IsChecked = false;
        LabelHeader.Text = $"Nowy punkt czasowy {m_Schedule.Date:dd.MM.yyyy} {m_Configuration.GetLongNameOfDayWeek(m_Schedule.Date)}";

		if(m_Event != null)
		{
            if (m_Event.StopTime != m_Event.StartTime)
            {
                CheckBoxStopTime.IsChecked = true;
            }

            IsEditing = true;
            LabelHeader.Text = $"Edycja puktu czasowego {m_Schedule.Date:dd.MM.yyyy} {m_Configuration.GetLongNameOfDayWeek(m_Schedule.Date)}";
			NameEditor.Text = m_Event.Name;
			StartDateEditor.Text = m_Event.StartTime.ToString("hh:mm");
			StopDateEditor.Text = m_Event.StopTime.ToString("hh:mm");
			DurationLabel.Text = $"{(m_Event.StartTime - m_Event.StopTime).Hours}h {(m_Event.StartTime - m_Event.StopTime).Minutes}m";
        }
    }

	public async void Submit(object sender, EventArgs e)
	{
		if(string.IsNullOrEmpty(NameEditor.Text))
		{
            await Shell.Current.CurrentPage.DisplayAlert("B³¹d", "Nazwa punktu harmoongramu nie mo¿e byæ pusta", "Popraw");
		}

        List<string> startTime = StartDateEditor.Text.Split(":").ToList();
        int hour = Int32.Parse(startTime[0]);
        int minutes = Int32.Parse(startTime[1]);
        if (startTime.Count != 2 || string.IsNullOrEmpty(startTime[0]) || string.IsNullOrEmpty(startTime[1]) 
            || hour < 0 || hour > 23 || minutes < 0 || minutes > 59)
        {
            await Shell.Current.CurrentPage.DisplayAlert("B³¹d", "B³êdnie ustawiony czas rozpoczêcia, wymagany format -> godzina:minuta np. 12:30", "Popraw");
        }

        int hour2 = 0;
        int minutes2 = 0;
        if (CheckBoxStopTime.IsChecked)
        {
            List<string> stopTime = StopDateEditor.Text.Split(":").ToList();
            hour2 = Int32.Parse(stopTime[0]);
            minutes2 = Int32.Parse(stopTime[1]);
            if (stopTime.Count != 2 || string.IsNullOrEmpty(stopTime[0]) || string.IsNullOrEmpty(stopTime[1])
                || hour2 < 0 || hour2 > 23 || minutes2 < 0 || minutes2 > 59)
            {
                await Shell.Current.CurrentPage.DisplayAlert("B³¹d", "B³êdnie ustawiony czas zakoczenia, wymagany format -> godzina:minuta np. 12:30", "Popraw");
            }
        }


        if (IsEditing)
		{
            CreateScheduleEventDTO createScheduleEventDTO = new CreateScheduleEventDTO
            {
                ScheduleDayId = m_Schedule.Id,
                Name = NameEditor.Text,
                StartTime = new DateTime(m_Schedule.Date.Year, m_Schedule.Date.Month, m_Schedule.Date.Day, hour, minutes, 0),
                StopTime = new DateTime(m_Schedule.Date.Year, m_Schedule.Date.Month, m_Schedule.Date.Day, hour, minutes, 0),
            };
            if(CheckBoxStopTime.IsChecked)
            {
                createScheduleEventDTO.StopTime = new DateTime(m_Schedule.Date.Year, m_Schedule.Date.Month, m_Schedule.Date.Day, hour2, minutes2, 0);
            }

			var response = await m_ScheduleService.CreateScheduleEvent(createScheduleEventDTO);
			if(response.Success) 
			{
                var confirmCopyToast = Toast.Make("Dodano nowy punkt do harmonogramu", ToastDuration.Short, 14);
                await confirmCopyToast.Show();
            }
            else
                await Shell.Current.CurrentPage.DisplayAlert("Uwaga", response.Message, "Ok");
        }	
		else
		{
            EditScheduleEventDTO editScheduleEventDTO = new EditScheduleEventDTO
            {
                Name = NameEditor.Text,
                StartTime = new DateTime(m_Schedule.Date.Year, m_Schedule.Date.Month, m_Schedule.Date.Day, hour, minutes, 0),
                StopTime = new DateTime(m_Schedule.Date.Year, m_Schedule.Date.Month, m_Schedule.Date.Day, hour, minutes, 0),
            };
            if (CheckBoxStopTime.IsChecked)
            {
                editScheduleEventDTO.StopTime = new DateTime(m_Schedule.Date.Year, m_Schedule.Date.Month, m_Schedule.Date.Day, hour2, minutes2, 0);
            }

            var response = await m_ScheduleService.UpdateScheduleEvent(m_Schedule.Id, m_Event.Id, m_Configuration.User.Id, editScheduleEventDTO);
            if (response.Success)
            {
                var confirmCopyToast = Toast.Make("Zmodyfikowano punkt harmonogramu", ToastDuration.Short, 14);
                await confirmCopyToast.Show();
            }
            else
                await Shell.Current.CurrentPage.DisplayAlert("Uwaga", response.Message, "Ok");
        }
    }
}