using TripPlanner.ViewModels.Schedule;

namespace TripPlanner.Views.ScheduleViews;

public partial class ScheduleListPage : ContentPage
{
	public ScheduleListPage(ScheduleListViewModel scheduleViewModel)
	{
		InitializeComponent();
		BindingContext= scheduleViewModel;
	}
}