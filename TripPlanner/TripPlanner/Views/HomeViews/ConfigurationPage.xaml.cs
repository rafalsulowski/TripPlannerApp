using TripPlanner.ViewModels.Home;

namespace TripPlanner.Views.HomeViews;

public partial class ConfigurationPage : ContentPage
{
	public ConfigurationPage(ConfigurationViewModel configurationViewModel)
	{
		InitializeComponent();
		BindingContext = configurationViewModel;
	}
}