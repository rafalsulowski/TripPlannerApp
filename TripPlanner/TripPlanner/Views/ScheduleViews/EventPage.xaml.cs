using TripPlanner.Models.DTO.ScheduleDTOs;
using TripPlanner.Services;
using TripPlanner.ViewModels.Schedule;

namespace TripPlanner.Views.ScheduleViews;

public partial class EventPage : ContentPage
{
    public EventPage(ScheduleEventViewModel scheduleEventViewModel)
	{
		InitializeComponent();
        BindingContext = scheduleEventViewModel;
    }
}