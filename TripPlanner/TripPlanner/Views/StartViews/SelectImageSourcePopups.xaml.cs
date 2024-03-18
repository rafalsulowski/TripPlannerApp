using CommunityToolkit.Maui.Views;
namespace TripPlanner.Views.StartViews;

public partial class SelectImageSourcePopups : Popup
{
	public SelectImageSourcePopups()
	{
		InitializeComponent();
	}

    async void GoBack(Object sender, EventArgs e)
    {
        await CloseAsync();
    }

    async void DoPicture(Object sender, EventArgs e)
    {
        await CloseAsync(1);
    }
    async void SelectFromGallery(Object sender, EventArgs e)
    {

        await CloseAsync(0);
    }
}