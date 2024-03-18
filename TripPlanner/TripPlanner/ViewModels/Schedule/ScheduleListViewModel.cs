using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Core.Extensions;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using TripPlanner.Models.DTO.ScheduleDTOs;
using TripPlanner.Services;
using CommunityToolkit.Maui.Storage;
using TripPlanner.Models.DTO.TourDTOs;
using System.Text;
using UglyToad.PdfPig.Writer;
using UglyToad.PdfPig.Content;
using UglyToad.PdfPig.Fonts.Standard14Fonts;
using UglyToad.PdfPig.Core;

namespace TripPlanner.ViewModels.Schedule
{
    public partial class ScheduleListViewModel : ObservableObject, IQueryAttributable
    {
        CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        IFileSaver m_FileSaver;
        private readonly TourService m_TourService;
        private readonly Configuration m_Configuration;
        private readonly ScheduleService m_ScheduleService;
        private int TourId;

        [ObservableProperty]
        ObservableCollection<ScheduleDayDTO> schedules;

        public ScheduleListViewModel(Configuration configuration, ScheduleService scheduleService, TourService tourService, IFileSaver fileSaver)
        {
            m_FileSaver = fileSaver;
            m_TourService = tourService;
            m_Configuration = configuration;
            m_ScheduleService = scheduleService;
            Schedules = new ObservableCollection<ScheduleDayDTO>();
        }

        public async void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            TourId = (int)query["passTourId"];
            await LoadData();
        }

        [RelayCommand]
        async Task GoBack()
        {
            var navigationParameter = new Dictionary<string, object>
            {
                { "passTourId",  TourId}
            };
            await Shell.Current.GoToAsync($"/Tour", navigationParameter);
        }

        [RelayCommand]
        async Task GoDay(ScheduleDayDTO day)
        {
            List<int> days = new List<int>();
            foreach(var dayy in Schedules)
            {
                days.Add(dayy.Id);
            }

            var navigationParameter = new Dictionary<string, object>
            {
                { "passTourId",  TourId},
                { "passScheduleDayId",  day.Id},
                { "passListDays",  days},
                { "IsFromCalendarPage", false }
            };
            await Shell.Current.GoToAsync($"/ScheduleDay", navigationParameter);
        }

        [RelayCommand]
        async Task GoCalendar()
        {
            var confirmCopyToast = Toast.Make("Funkcjonalność niezaimplementowana!", ToastDuration.Long, 14);
            await confirmCopyToast.Show();
        }

        [RelayCommand]
        async Task Export()
        {
            TourDTO tour = await m_TourService.GetTourToPdfById(TourId);
            List<ExtendParticipantDTO> participants = await m_TourService.GetTourExtendParticipant(TourId);
            if (tour == null || participants == null)
                await Shell.Current.CurrentPage.DisplayAlert("Uwaga", $"Nie można wygenerować harmonogramu ze względu na brak danych wyieczki", "Ok");


            PdfDocumentBuilder builder = new PdfDocumentBuilder();
            List<PdfPageBuilder> pages = new List<PdfPageBuilder> { builder.AddPage(PageSize.A4) };

            PdfDocumentBuilder.AddedFont fontBold = builder.AddStandard14Font(Standard14Font.HelveticaBold);
            PdfDocumentBuilder.AddedFont fontRegular = builder.AddStandard14Font(Standard14Font.Helvetica);

            try
            {
                int xH = 75;
                pages[0].AddText(tour.Title, 22, new PdfPoint(xH, 750), fontBold);

                string dates = $"{tour.TargetRegion} {tour.StartDate:dd.MM.yyyy} - {tour.EndDate:dd.MM.yyyy}" +
                    $" ({m_Configuration.GetLongNameOfDayWeek(tour.StartDate)} - {m_Configuration.GetLongNameOfDayWeek(tour.EndDate)})";
                pages[0].AddText(m_Configuration.normalizeString(dates), 12, new PdfPoint(xH, 735), fontRegular);

                int countOgranizers = participants.Count(u => u.IsOrganizer);
                int count = 0;
                string organizers = "Organizatorzy: ";
                for (int i = 0; i < participants.Count; i++)
                {
                    if (participants[i].IsOrganizer)
                    {
                        organizers += string.IsNullOrEmpty(participants[i].Nickname) ? participants[i].FullName : participants[i].Nickname;
                        count++;
                        if (count < countOgranizers)
                            organizers += ", ";
                    }
                }
                pages[0].AddText(Encoding.UTF8.GetString(Encoding.GetEncoding("ASCII").GetBytes(organizers)), 14, new PdfPoint(xH, 695), fontRegular);
                pages[0].AddText("Szczegolowy harmonogram wyjazdu", 14, new PdfPoint(xH, 650), fontBold);

                int pageIt = 0;
                int y = 625;
                int x1 = xH;
                int x2 = xH + 25;
                List<ScheduleDayDTO> SchedulesList = tour.Schedule.ToList();
                for (int i = 0; i < SchedulesList.Count; i++)
                {
                    pages[pageIt].AddText($"{i + 1}. {m_Configuration.GetLongNameOfDayWeek(SchedulesList[i].Date)} {SchedulesList[i].Date:dd.MM.yyyy}", 12, new PdfPoint(x1, y), fontRegular);
                    y -= 20;
                    if (y <= 55)
                    {
                        pageIt++;
                        pages.Add(builder.AddPage(PageSize.A4));
                        y = 775;
                    }
                    if (SchedulesList[i].Events.Count == 0)
                    {
                        pages[pageIt].AddText($"(brak zaplanowanych wydarzen)", 12, new PdfPoint(x2, y), fontRegular);
                        y -= 25;
                    }
                    else//Encoding.UTF8.GetString(Encoding.GetEncoding("ASCII").GetBytes(SchedulesList[i].Events.ElementAt(j).Name))
                    {
                        for (int j = 0; j < SchedulesList[i].Events.Count; j++)
                        {
                            if (SchedulesList[i].Events.ElementAt(j).StartTime == SchedulesList[i].Events.ElementAt(j).StopTime)
                                pages[pageIt].AddText($"{SchedulesList[i].Events.ElementAt(j).StartTime:HH:mm} - {SchedulesList[i].Events.ElementAt(j).StartTime:HH:mm}   {m_Configuration.normalizeString(SchedulesList[i].Events.ElementAt(j).Name)}", 12, new PdfPoint(x2, y), fontRegular);
                            else
                                pages[pageIt].AddText($"{SchedulesList[i].Events.ElementAt(j).StartTime:HH:mm}   {m_Configuration.normalizeString(SchedulesList[i].Events.ElementAt(j).Name)}", 12, new PdfPoint(x2, y), fontRegular);
                            y -= 20;
                        }
                        y -= 5;
                    }
                    if (y <= 55)
                    {
                        pageIt++;
                        pages.Add(builder.AddPage(PageSize.A4));
                        y = 775;
                    }
                }
            }
            catch (Exception)
            {
                await Shell.Current.CurrentPage.DisplayAlert("Uwaga", $"Nie udało się zapisać harmonogramu, błędy w zapisie informacji do pliku", "Ok");
            }

            byte[] documentBytes = builder.Build();
            using var stream = new MemoryStream(documentBytes); 
            string filename = $"Harmonogram_{m_Configuration.GetTourTitleWithoutWhiteSpacesAndPolishCharacter(tour.Title)}.pdf";
            var documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string path = Path.Combine(documents, filename);

            try
            {
                var res = await m_FileSaver.SaveAsync(path, stream, cancellationTokenSource.Token);
            }
            catch(Exception)
            {
                await Shell.Current.CurrentPage.DisplayAlert("Uwaga", $"Nie udało się zapisać harmonogramu, błąd w zapisie pliku", "Ok");
            }
        }

        private async Task LoadData()
        {
            var result = await m_ScheduleService.GetSchedule(TourId);
            Schedules = result.ToObservableCollection();
        }
    }
}
