using TripPlanner.ViewModels.Shares;

namespace TripPlanner.Views.ShareViews;

public partial class CreateBillPage : ContentPage
{
	public CreateBillPage(CreateBillViewModel createBillViewModel)
	{
		InitializeComponent();
		BindingContext = createBillViewModel;
    }
}