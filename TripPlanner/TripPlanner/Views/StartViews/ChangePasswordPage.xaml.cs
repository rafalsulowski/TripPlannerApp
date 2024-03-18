using TripPlanner.ViewModels.User;

namespace TripPlanner.Views.StartViews;

public partial class ChangePasswordPage : ContentPage
{
	public ChangePasswordPage(ChangePasswordViewModel changePasswordViewModel)
	{
		InitializeComponent();
		BindingContext = changePasswordViewModel;
	}
}