using TripPlanner.ViewModels.Schedule;

namespace TripPlanner.Views.ScheduleViews;

public partial class ScheduleCalendarPage : ContentPage
{
	public ScheduleCalendarPage(ScheduleCalendarViewModel scheduleCalendarViewModel)
	{
		InitializeComponent();
		BindingContext= scheduleCalendarViewModel;
	}
}