using TripPlanner.Views.ChatViews;
using TripPlanner.Views.CheckListViews;
using TripPlanner.Views.FriendViews;
using TripPlanner.Views.HomeViews;
using TripPlanner.Views.MapsViews;
using TripPlanner.Views.ParticipantViews;
using TripPlanner.Views.ScheduleViews;
using TripPlanner.Views.ShareViews;
using TripPlanner.Views.StartViews;
using TripPlanner.Views.TourViews;
using TripPlanner.Views.WeatherViews;

namespace TripPlanner;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();

        Routing.RegisterRoute("Start", typeof(StartPage));
        Routing.RegisterRoute("Start/ChangePassword", typeof(ChangePasswordPage));
        Routing.RegisterRoute("Start/CheckEmail", typeof(CheckYourEmailPage));
        Routing.RegisterRoute("Start/Login", typeof(LoginPage));
        Routing.RegisterRoute("Start/Register", typeof(RegisterPage));
        Routing.RegisterRoute("Home", typeof(HomePage));
		Routing.RegisterRoute("CreateTour", typeof(CreateTour));
		Routing.RegisterRoute("Calendar", typeof(CalendarPage));
		Routing.RegisterRoute("Notifications", typeof(NotificationPage));
		Routing.RegisterRoute("Notifications/NotificationDetails", typeof(NotificationDetailsPage));
		Routing.RegisterRoute("Profile", typeof(ProfilePage));
		Routing.RegisterRoute("Profile/Details", typeof(DetailsAndEditUserDatePage));
		Routing.RegisterRoute("Profile/Settings", typeof(ConfigurationPage));
		Routing.RegisterRoute("Friends", typeof(FriendsPage));
		Routing.RegisterRoute("Friends/AddFriend", typeof(AddFriendPage));
		Routing.RegisterRoute("Tour", typeof(TourPage));
		Routing.RegisterRoute("Tour/Chat", typeof(ChatPage));
		Routing.RegisterRoute("Tour/Chat/CreateQuestionnaire", typeof(CreateNewQuestionnairePage));
		Routing.RegisterRoute("Tour/Participants", typeof(ParticipantsListPage));
		Routing.RegisterRoute("Tour/Participants/AddParticipantFromFriends", typeof(AddParticipantPage));
		Routing.RegisterRoute("Tour/Shares", typeof(SharesListPage));
		Routing.RegisterRoute("Tour/Shares/CreateBill", typeof(CreateBillPage));
		Routing.RegisterRoute("Tour/Shares/CreateBill/DivisionType", typeof(DivisionTypePage));
		Routing.RegisterRoute("Tour/Shares/Bill", typeof(BillPage));
		Routing.RegisterRoute("Tour/Shares/CreateTransferSelect", typeof(CreateTransferSelectPage));
		Routing.RegisterRoute("Tour/Shares/CreateTransferSelect/CreateTransferSubmit", typeof(CreateTransferSubmitPage));	
		Routing.RegisterRoute("Tour/Shares/Transfer", typeof(TransferPage));
		Routing.RegisterRoute("Tour/Shares/Balance", typeof(BalancePage));
		Routing.RegisterRoute("Tour/ScheduleList", typeof(ScheduleListPage));
		Routing.RegisterRoute("Tour/ScheduleCalendar", typeof(ScheduleCalendarPage));
		Routing.RegisterRoute("Tour/ScheduleDay", typeof(ScheduleDayPage));
		Routing.RegisterRoute("Tour/ScheduleDay/Event", typeof(EventPage));
		Routing.RegisterRoute("Tour/Weather", typeof(WeatherPage));
		Routing.RegisterRoute("Tour/CheckLists", typeof(CheckListsPage));
		Routing.RegisterRoute("Tour/CheckLists/CreateCheckList", typeof(CreateCheckListPage));
		Routing.RegisterRoute("Tour/CheckLists/CheckList", typeof(CheckListPage));
		Routing.RegisterRoute("Tour/CheckLists/Questionnaire", typeof(QuestionnairePage));
		Routing.RegisterRoute("Tour/LocationsMap", typeof(LocationsMapPage));
	}	
}
