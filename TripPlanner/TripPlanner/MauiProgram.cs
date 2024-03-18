using TripPlanner.Views.HomeViews;
using TripPlanner.Views.StartViews;
using TripPlanner.Views.TourViews;
using TripPlanner.Services;
using TripPlanner.Views.ChatViews;
using CommunityToolkit.Maui;
using TripPlanner.DataTemplates;
using TripPlanner.Views.ParticipantViews;
using TripPlanner.Views.ShareViews;
using TripPlanner.ViewModels.Shares;
using TripPlanner.ViewModels.Tour;
using TripPlanner.ViewModels.Chat;
using TripPlanner.ViewModels.Participant;
using TripPlanner.Views.ScheduleViews;
using TripPlanner.ViewModels.Schedule;
using TripPlanner.ViewModels.Home;
using TripPlanner.ViewModels.User;
using TripPlanner.Views.WeatherViews;
using TripPlanner.ViewModels.Weather;
using TripPlanner.ViewModels.CheckList;
using TripPlanner.Views.CheckListViews;
using TripPlanner.ViewModels.Friend;
using TripPlanner.Views.FriendViews;
using CommunityToolkit.Maui.Storage;
using TripPlanner.ViewModels.Map;
using TripPlanner.Views.MapsViews;
using TripPlanner.ViewModels;

namespace TripPlanner;

public static class MauiProgram
{
	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
//#if ANDROID
			//.UseGoogleMaps()
//#elif IOS
//			.UseGoogleMaps()
//#endif
            .UseMauiCommunityToolkit()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
				fonts.AddFont("Roboto-Regular.ttf", "RobotoRegular");
			});
			

        //App Services
        builder.Services.AddSingleton<Configuration>();

		builder.Services.AddHttpClient("httpClient")
			.ConfigurePrimaryHttpMessageHandler(() => {
                HttpClientHandler handler = new HttpClientHandler();
				handler.AllowAutoRedirect = false;
				handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) =>
				{
					if (cert.Issuer.Equals("CN=localhost"))
						return true;

                    return true;
                    //return errors == System.Net.Security.SslPolicyErrors.None;
				};
				return handler;
			})
			.ConfigureHttpClient(u =>
			{
				u.Timeout = TimeSpan.FromSeconds(10); //bezpieczna wartosc dla wszystkich zapytan (roweniz tych z emialem)
			});

        //Data Template Selectors
        builder.Services.AddSingleton<MessageDataTemplateSelector>();
        builder.Services.AddSingleton<FriendDataTemplateSelector>();
        builder.Services.AddSingleton<ParticipantDataTemplateSelector>();

        //Other Services
        builder.Services.AddSingleton<IFileSaver>(FileSaver.Default);

        //Data Managment Services
        builder.Services.AddSingleton<TourService>();
        builder.Services.AddSingleton<ChatService>();
        builder.Services.AddSingleton<UserService>();
        builder.Services.AddSingleton<ShareService>();
        builder.Services.AddSingleton<ScheduleService>();
        builder.Services.AddSingleton<WeatherFastService>();
        builder.Services.AddSingleton<CheckListService>();
        builder.Services.AddSingleton<NotificationService>();


        //ViewModels
        builder.Services.AddTransient<HomeViewModel>();
		builder.Services.AddTransient<CreateTourViewModel>();
		builder.Services.AddTransient<CalendarViewModel>();
        builder.Services.AddSingleton<NotificationViewModel>();
        builder.Services.AddSingleton<NotificationDetailsViewModel>();
		builder.Services.AddTransient<FriendsViewModel>();
		builder.Services.AddTransient<AddFriendViewModel>();
		builder.Services.AddTransient<ProfileViewModel>();
		builder.Services.AddTransient<TourViewModel>();
		builder.Services.AddTransient<ChatViewModel>();
		builder.Services.AddTransient<CreateQuestionnaireViewModel>();
		builder.Services.AddTransient<QuestionnaireViewModel>();
		builder.Services.AddTransient<ParticipantsViewModel>();
		builder.Services.AddTransient<AddParticipantsViewModel>();
		builder.Services.AddTransient<SharesViewModel>();
		builder.Services.AddTransient<CreateBillViewModel>();
		builder.Services.AddTransient<DivisionTypeViewModel>();
		builder.Services.AddTransient<BillViewModel>();
		builder.Services.AddTransient<CreateTransferSelectViewModel>();
		builder.Services.AddTransient<CreateTransferSubmitViewModel>();
		builder.Services.AddTransient<TransferViewModel>();
		builder.Services.AddTransient<BalanceViewModel>();
		builder.Services.AddTransient<ScheduleCalendarViewModel>();
		builder.Services.AddTransient<ScheduleDayViewModel>();
		builder.Services.AddTransient<ScheduleListViewModel>();
		builder.Services.AddTransient<ScheduleEventViewModel>();
		builder.Services.AddTransient<LoginViewModel>();
		builder.Services.AddTransient<RegisterViewModel>();
		builder.Services.AddTransient<WeatherViewModel>();
		builder.Services.AddTransient<CheckListsViewModel>();
		builder.Services.AddTransient<CreateCheckListViewModels>();
		builder.Services.AddTransient<CheckListViewModel>();
		builder.Services.AddTransient<QuestionnaireStandAloneViewModel>();
		builder.Services.AddTransient<ProfileDetailsViewModel>();
		builder.Services.AddTransient<ChangePasswordViewModel>();
		builder.Services.AddTransient<ConfigurationViewModel>();
		builder.Services.AddTransient<LocationsMapViewModel>();


        //Views
        builder.Services.AddTransient<StartPage>();
		builder.Services.AddTransient<HomePage>();
		builder.Services.AddTransient<CreateTour>();
		builder.Services.AddTransient<CalendarPage>();
		builder.Services.AddTransient<NotificationPage>();
		builder.Services.AddTransient<NotificationDetailsPage>();
		builder.Services.AddTransient<ProfilePage>();
		builder.Services.AddTransient<FriendsPage>();
		builder.Services.AddTransient<AddFriendPage>();
		builder.Services.AddTransient<TourPage>();
		builder.Services.AddTransient<ChatPage>();
		builder.Services.AddTransient<CreateNewQuestionnairePage>();
		builder.Services.AddTransient<ParticipantsListPage>();
		builder.Services.AddTransient<AddParticipantPage>();
		builder.Services.AddTransient<SharesListPage>();
		builder.Services.AddTransient<CreateBillPage>();
		builder.Services.AddTransient<DivisionTypePage>();
		builder.Services.AddTransient<BillPage>();
		builder.Services.AddTransient<CreateTransferSelectPage>();
		builder.Services.AddTransient<CreateTransferSubmitPage>();
		builder.Services.AddTransient<TransferPage>();
		builder.Services.AddTransient<BalancePage>();
		builder.Services.AddTransient<ScheduleListPage>();
		builder.Services.AddTransient<ScheduleCalendarPage>();
		builder.Services.AddTransient<ScheduleDayPage>();
		builder.Services.AddTransient<EventPage>();
		builder.Services.AddTransient<LoginPage>();
		builder.Services.AddTransient<RegisterPage>();
		builder.Services.AddTransient<WeatherPage>();
		builder.Services.AddTransient<CheckListsPage>();
		builder.Services.AddTransient<CheckListPage>();
		builder.Services.AddTransient<CreateCheckListPage>();
		builder.Services.AddTransient<QuestionnairePage>();
		builder.Services.AddTransient<DetailsAndEditUserDatePage>();
		builder.Services.AddTransient<ChangePasswordPage>();
		builder.Services.AddTransient<CheckYourEmailPage>();
		builder.Services.AddTransient<ConfigurationPage>();
		builder.Services.AddTransient<LocationsMapPage>();


		return builder.Build();
	}
}
