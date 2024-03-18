using CommunityToolkit.Maui.Views;
using TripPlanner.ViewModels.CheckList;

namespace TripPlanner.Views.ChatViews;

public partial class AddCheckListFieldPopups : Popup
{
	private readonly List<Tuple2String> Fields;

    public AddCheckListFieldPopups(List<Tuple2String> fields)
	{
		InitializeComponent();
        Fields = fields;
    }

	async void Submit_Clicked(Object sender, EventArgs e)
	{
		if (string.IsNullOrEmpty(m_Answer.Text))
		{
			await Shell.Current.CurrentPage.DisplayAlert("B³¹d", "Nazwa nie mo¿e byæ pusta", "Popraw");
            return;
        }

        if (string.IsNullOrEmpty(m_Multiplicity.Text))
        {
            var res = await Shell.Current.CurrentPage.DisplayAlert("B³¹d", "Pole krotnoœci przedmiotu jest pustê, czy chcesz kontunuowaæ?", "Tak", "Nie");
			if (!res)
				return;
        }
        
		if (Fields.FirstOrDefault(u => u.Name == m_Answer.Text) != null)
		{
			var res = await Shell.Current.CurrentPage.DisplayAlert("Uwaga", "Istnieje pozycja o takiej nazwie, czy chcesz kontunuowaæ?", "Tak", "Nie");
			if(res)
                await CloseAsync(new Tuple2String { Name = m_Answer.Text, Multiplicity = m_Multiplicity.Text });
            else
                return;
        }
        else
            await CloseAsync(new Tuple2String { Name = m_Answer.Text, Multiplicity = m_Multiplicity.Text });
    }
}