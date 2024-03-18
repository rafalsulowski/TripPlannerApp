using TripPlanner.ViewModels.Shares;

namespace TripPlanner.Views.ShareViews;

public partial class CreateTransferSubmitPage : ContentPage
{
	public CreateTransferSubmitPage(CreateTransferSubmitViewModel createTransferSubmitViewModel)
	{
		InitializeComponent();
        BindingContext = createTransferSubmitViewModel;
    }
}