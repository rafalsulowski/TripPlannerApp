using TripPlanner.ViewModels.Shares;

namespace TripPlanner.Views.ShareViews;

public partial class SharesListPage : ContentPage
{
	public SharesListPage(SharesViewModel sharesViewModel)
	{
		InitializeComponent();
		BindingContext = sharesViewModel;
    }
}