using TripPlanner.ViewModels.Shares;

namespace TripPlanner.Views.ShareViews;

public partial class BillPage : ContentPage
{
	public BillPage(BillViewModel billViewModel)
	{
		InitializeComponent();
		BindingContext = billViewModel;
	}
}