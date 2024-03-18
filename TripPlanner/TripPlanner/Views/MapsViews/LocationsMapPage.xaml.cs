using TripPlanner.ViewModels.Map;

namespace TripPlanner.Views.MapsViews;

public partial class LocationsMapPage : ContentPage
{
	public LocationsMapPage(LocationsMapViewModel locationsMapViewModel)
	{
		InitializeComponent();
		BindingContext = locationsMapViewModel;

    }
}