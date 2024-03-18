using TripPlanner.ViewModels.Shares;

namespace TripPlanner.Views.ShareViews;

public partial class BalancePage : ContentPage
{
	public BalancePage(BalanceViewModel balanceViewModel)
	{
		InitializeComponent();
		BindingContext = balanceViewModel;
	}
}