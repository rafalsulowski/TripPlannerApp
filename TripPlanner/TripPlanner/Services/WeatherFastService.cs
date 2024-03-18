using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Reflection.Metadata;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using TripPlanner.Models.DTO.TourDTOs;
using TripPlanner.Models.Models;

namespace TripPlanner.Services
{
    public class WeatherModel
    {
        //Main data
        public string Location { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string Icon { get; set; } = string.Empty;
        public double Temperature { get; set; }
        public double TemperatureFeels { get; set; }
        public string Description { get; set; } = string.Empty;
        public DateTime Sunrise { get; set; }
        public DateTime Sunset { get; set; }


        //Collections
        public List<SingleWeahterModel> Hours { get; set; } = new List<SingleWeahterModel>();
        public List<SingleWeahterModel> Days { get; set; } = new List<SingleWeahterModel>();


        //Details
        public DateTime CreatedDate { get; set; }
        public int Humidity { get; set; } //wilgotnosc
        public int PrecipProb { get; set; } //szansa na opady
        public int Precip { get; set; } //ilość opadów
        public bool Snow { get; set; } //czy będzie śnieg (TAK/NIE)
        public double WindSpeed { get; set; }
        public int WindDir { get; set; }
        public int UvIndex { get; set; }
        public int MoonPhase { get; set; } //faza ksiezyca
        public double Pressure { get; set; } //cisnienie w hpa
        public double Visibility { get; set; } //przejrzstosc w km
        public double Cloudcover { get; set; } //zachmurzenie nieba w procentach

    }

    public class SingleWeahterModel
    {
        public string Date { get; set; }
        public string Icon { get; set; } = string.Empty;
        public double Temperature { get; set; }
        public int PrecipProb { get; set; } //szansa na opady
        public int PrecipSize { get; set; } //ilość opadów
    }



    public class WeatherFastService
    {
        private readonly HttpClient m_HttpClient;
        private readonly Configuration m_Configuration;
        private readonly TourService m_TourService;

        public WeatherFastService(IHttpClientFactory httpClient, Configuration configuration, TourService tourService)
        {
            m_HttpClient = httpClient.CreateClient("httpClient");
            m_Configuration = configuration;
            m_TourService = tourService;
        }


        // Zwraca dane o pogodzie na 14 dni
        public async Task<RepositoryResponse<WeatherModel>> GetWeatherOn14Days(int tourId)
        {
            string errMsg = "";
            try
            {
                TourDTO tour = await m_TourService.GetTourById(tourId);

                if(tour == null || string.IsNullOrEmpty(tour.WeatherCords))
                    errMsg = "Brak danych o wycieczce oraz lokalizacji pogody";
                else
                {
                    DateTime startDate = DateTime.Now;
                    DateTime stopDate = DateTime.Now + new TimeSpan(m_Configuration.WeatherDaysForecast,0,0,0,0,0);

                    HttpResponseMessage response = m_HttpClient.GetAsync($"{m_Configuration.WeatherApiUrl}/{tour.WeatherCords}/{startDate:yyyy-MM-dd}/{stopDate:yyyy-MM-dd}?unitGroup=metric&iconSet=icons2&include=hours%2Ccurrent&key={m_Configuration.WeatherApiKey}&contentType=json").Result;
                    if (response.IsSuccessStatusCode)
                    {
                        var resp = await response.Content.ReadAsStringAsync();
                        if (resp != null)
                        {
                            WeatherModel weather = new WeatherModel();
                            dynamic json = JsonConvert.DeserializeObject(resp);

                            //current weather
                            var current = json.currentConditions;

                            //main
                            weather.Location = json.resolvedAddress;
                            weather.Date = DateTime.Now;
                            weather.Icon = ((string)current.icon).Replace('-','_') + ".png";
                            weather.Temperature = current.temp;
                            weather.TemperatureFeels = current.feelslike;
                            weather.Description = GetWeatherDesc((string)current.icon); //w zaleznosci od icon
                            weather.Sunrise = current.sunrise;
                            weather.Sunset = current.sunset;

                            //details
                            weather.CreatedDate = current.datetime;
                            weather.Humidity = current.humidity;
                            weather.PrecipProb = current.precipprob;
                            weather.Precip = current.precip;
                            weather.Snow = current.snow == 0 ? false : true;
                            weather.WindSpeed = current.windspeed;
                            weather.WindDir = current.winddir;
                            weather.UvIndex = current.uvindex;
                            weather.Cloudcover = current.cloudcover;
                            weather.MoonPhase = current.moonphase;
                            weather.Pressure = current.pressure;
                            weather.Visibility = current.visibility;

                            //Days
                            dynamic days = json.days;
                            for(int i = 0; i < days.Count; i++)
                            {
                                weather.Days.Add(new SingleWeahterModel 
                                {
                                    Date= ((DateTime)days[i].datetime).ToString("dd.MM"),
                                    Icon = ((string)days[i].icon).Replace('-','_') + ".png",
                                    PrecipProb = days[i].precipprob,
                                    PrecipSize = days[i].precip,
                                    Temperature = days[i].temp,
                                });

                                if (((DateTime)days[i].datetime).DayOfYear == DateTime.Now.DayOfYear)
                                    weather.Days[i].Date = "Dziś";
                            }

                            //Hours for now to midnight
                            int counter = 0;
                            dynamic hours = days[0].hours;
                            for (int i = DateTime.Now.Hour; i < hours.Count; i++)
                            {
                                weather.Hours.Add(new SingleWeahterModel
                                {
                                    Date = ((DateTime)hours[i].datetime).ToString("HH:00"),
                                    Icon = ((string)hours[i].icon).Replace('-', '_') + ".png",
                                    PrecipProb = hours[i].precipprob,
                                    PrecipSize = hours[i].precip,
                                    Temperature = hours[i].temp,
                                });

                                if (((DateTime)hours[i].datetime).Hour == DateTime.Now.Hour)
                                    weather.Hours[weather.Hours.Count - 1].Date = "Teraz";

                                counter++;
                            }

                            //Hours for midnight to weather.Hours.Count == 24
                            hours = days[1].hours;
                            int counts = 24 - counter;
                            for (int i = 0; i < counts; i++)
                            {
                                weather.Hours.Add(new SingleWeahterModel
                                {
                                    Date = ((DateTime)hours[i].datetime).ToString("HH:00"),
                                    Icon = "Weather/" + ((string)hours[i].icon).Replace('-', '_') + ".png",
                                    PrecipProb = hours[i].precipprob,
                                    PrecipSize = hours[i].precip,
                                    Temperature = hours[i].temp,
                                });
                            }

                            return new RepositoryResponse<WeatherModel> { Data = weather, Message = "", Success = true };
                        }
                        else
                            errMsg = "Brak danych o pogodzie";
                    }
                    else
                        errMsg = $"Kod błędu: {response.StatusCode}";
                }
            }
            catch (Exception e)
            {
                if (e.InnerException is TaskCanceledException)
					errMsg = "Przepraszamy, serwer poza zasięgiem, sprawdź swoje połączenie internetowe lub spróbuj później";
				else
					errMsg = $"Wyjątek: {e.Message}";
            }
            return new RepositoryResponse<WeatherModel> { Data = null, Message = errMsg, Success = false };
        }

        public string GetWeatherDesc(string iconName)
        {
            switch(iconName)
            {
                case "snow":
                    return "Opady śniegu";
                case "snow-showers-day":
                    return "Okresowe opady śniegu";
                case "snow-showers-night":
                    return "W nocy okresowe opady śniegu";
                case "thunder-rain":
                    return "Burza z deszczem";
                case "thunder-showers-day":
                    return "Możliwe burze";
                case "thunder-showers-night":
                    return "W nocy możliwe burze";
                case "rain":
                    return "Deszcz";
                case "showers-day":
                    return "Przelotne opady";
                case "showers-night":
                    return "W nocy przelotne opady";
                case "fog":
                    return "Mgła";
                case "wind":
                    return "Porywisty wiatr";
                case "cloudy":
                    return "Zachmurzenie całkowite";
                case "partly-cloudy-day":
                    return "Zachmurzenie częsciowe";
                case "partly-cloudy-night":
                    return "W nocy zachmurzenie częściowe";
                case "clear-day":
                    return "Słonecznie";
                case "clear-night":
                    return "Bezchmurna noc";
                default:
                    return "Słonecznie";
            }
        }
    }
}
