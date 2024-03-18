using TripPlanner.ViewModels.User;

namespace TripPlanner.Views.StartViews;

public partial class RegisterPage : ContentPage
{
	public RegisterPage(RegisterViewModel registerViewModel)
	{
		InitializeComponent();
		BindingContext = registerViewModel;
	}
}