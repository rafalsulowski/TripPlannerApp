using TripPlanner.ViewModels.Shares;

namespace TripPlanner.Views.ShareViews;

public partial class CreateTransferSelectPage : ContentPage
{
	public CreateTransferSelectPage(CreateTransferSelectViewModel createTransferViewModel)
	{
		InitializeComponent();
		BindingContext = createTransferViewModel;
	}
}