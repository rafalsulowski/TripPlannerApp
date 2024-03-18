using TripPlanner.ViewModels;
using TripPlanner.ViewModels.Chat;

namespace TripPlanner.Views.ChatViews;

public partial class CreateNewQuestionnairePage : ContentPage
{
	public CreateNewQuestionnairePage(CreateQuestionnaireViewModel createQuestionnaireViewModel)
	{
		InitializeComponent();
        BindingContext = createQuestionnaireViewModel;
    }
}