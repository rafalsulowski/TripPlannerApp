using TripPlanner.ViewModels.CheckList;

namespace TripPlanner.Views.CheckListViews;

public partial class QuestionnairePage : ContentPage
{
	public QuestionnairePage(QuestionnaireStandAloneViewModel questionnaireStandAloneViewModel)
	{
		InitializeComponent();
		BindingContext = questionnaireStandAloneViewModel;
	}
}