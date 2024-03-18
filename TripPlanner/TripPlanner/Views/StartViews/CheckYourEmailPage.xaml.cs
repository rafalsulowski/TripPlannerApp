namespace TripPlanner.Views.StartViews;

public partial class CheckYourEmailPage : ContentPage, IQueryAttributable
{
	public CheckYourEmailPage()
	{
		InitializeComponent();
        Name.Text = "Na tw�j e-mail zosta� wys�ana wiadomo�� z linkiem do aktywacji konta";
    }

    public async void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        Name.Text = (string)query["passNote"];
    }

    public async void GoLogin(object e, EventArgs args)
    {
        await Shell.Current.GoToAsync("/Login");
    }
}