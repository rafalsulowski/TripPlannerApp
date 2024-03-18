using TripPlanner.ViewModels.Shares;

namespace TripPlanner.Views.ShareViews;

public partial class TransferPage : ContentPage
{
	public TransferPage(TransferViewModel transferViewModel)
	{
		InitializeComponent();
		BindingContext = transferViewModel;
	}
}