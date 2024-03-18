using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Core.Extensions;
using CommunityToolkit.Maui.Views;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TripPlanner.Models.DTO.CheckListDTOs;
using TripPlanner.Services;
using TripPlanner.Views.ChatViews;
using static System.Net.Mime.MediaTypeNames;

namespace TripPlanner.ViewModels.CheckList
{
    public partial class CheckListViewModel : ObservableObject, IQueryAttributable
    {
        private readonly CheckListService m_CheckListService;
        private readonly TourService m_TourService;
        private readonly Configuration m_Configuration;
        private HubConnection m_Connection;
        private int TourId;
        private int CheckListId;

        [ObservableProperty]
        CheckListDTO checkList;

        [ObservableProperty]
        bool isOrganizer;

        public CheckListViewModel(Configuration configuration, CheckListService checkListService, TourService tourService)
        {
            m_CheckListService = checkListService;
            m_Configuration = configuration;
            m_TourService = tourService;
            IsOrganizer = false;
        }

        public async void ApplyQueryAttributes(IDictionary<string, object> query)
        {
            TourId = (int)query["passTourId"];
            CheckListId = (int)query["passCheckListId"];
            await Connect();
        }

        async Task Connect()
        {
            var res = await m_TourService.GetTourExtendParticipantById(TourId, m_Configuration.User.Id);
            if (res is null)
            {
                await Shell.Current.CurrentPage.DisplayAlert("Błąd", "Nie jesteś już uczestnikiem tej wycieczki", "Ok");
                await GoBack();
            }
            else
                IsOrganizer = res.IsOrganizer;

            try
            {
                m_Connection = new HubConnectionBuilder()
                .WithUrl(m_Configuration.WssCheckListUrl)
                .Build();

                //gdy ktos doda pole do checklisty
                m_Connection.On<string>("CheckListFieldAddReceived", (message) =>
                {
                    CheckListFieldDTO field = JsonConvert.DeserializeObject<CheckListFieldDTO>(message);
                    if (field.CheckListId == CheckListId)
                    {
                        CheckList.Fields.Add(field);
                        //MainThread.BeginInvokeOnMainThread(() =>
                        //{
                        //    var confirmCopyToast = Toast.Make("Dodano pole do checklisty", ToastDuration.Short, 14);
                        //    confirmCopyToast.Show();
                        //});
                    }
                });

                //gdy ktos usunie pole z checklisty
                m_Connection.On<string>("CheckListFieldDeleteReceived", (message) =>
                {
                    int fieldId = Int32.Parse(JsonConvert.DeserializeObject<string>(message));
                    var elem = CheckList.Fields.FirstOrDefault(u => u.Id == fieldId);
                    if (elem != null)
                    {
                        CheckList.Fields.Remove(elem);
                        //MainThread.BeginInvokeOnMainThread(() =>
                        //{
                        //    var confirmCopyToast = Toast.Make($"Usunięto pole z checklisty", ToastDuration.Short, 14);
                        //    confirmCopyToast.Show();
                        //});
                    }
                });

                //gdy ktos zaznaczy lub odznaczy pole na checkliscie
                m_Connection.On<string>("CheckListFieldCheckReceived", (message) =>
                {
                    CheckListFieldDTO field = JsonConvert.DeserializeObject<CheckListFieldDTO>(message);
                    var elem = CheckList.Fields.FirstOrDefault(u => u.Id == field.Id);
                    if (elem != null)
                    {
                        elem.IsChecked = field.IsChecked;
                        //MainThread.BeginInvokeOnMainThread(() =>
                        //{
                        //    var confirmCopyToast = Toast.Make("Zaktualizowano pole checklisty", ToastDuration.Short, 14);
                        //    confirmCopyToast.Show();
                        //});
                    }
                });

                //gdy ktos usunie checkliste
                m_Connection.On<string>("CheckListDeleteReceived", (message) =>
                {
                    int checkListId = Int32.Parse(JsonConvert.DeserializeObject<string>(message));
                    if (CheckListId == checkListId)
                    {
                        MainThread.BeginInvokeOnMainThread( () => Shell.Current.CurrentPage.DisplayAlert("Uwaga", $"Checklista została usunięta", "Ok"));
                        MainThread.BeginInvokeOnMainThread(GoBackSynch);
                    }
                });

                //gdy ktos zmieni checkliste na prywatna
                m_Connection.On<string>("CheckListChangeVisibilityToPrivateReceived", (message) =>
                {
                    int checkListId = Int32.Parse(JsonConvert.DeserializeObject<string>(message));
                    if (CheckListId == checkListId)
                    {
                        CheckList.IsPublic = false;
                        if(m_Configuration.User.Id != CheckList.UserId)
                        {
                            MainThread.BeginInvokeOnMainThread(() =>
                            {
                                Shell.Current.CurrentPage.DisplayAlert("Uwaga", $"Checklista, którą właśnie oglądasz stała się prywatna", "Ok");
                                GoBackSynch();
                            });
                        }
                    }
                });

                //gdy ktos zmieni checkliste na publiczna
                m_Connection.On<string>("CheckListChangeVisibilityToPublicReceived", (message) =>
                {
                    int checkListId = Int32.Parse(JsonConvert.DeserializeObject<string>(message));
                    if (CheckListId == checkListId)
                    {
                        CheckList.IsPublic = true; 
                        //MainThread.BeginInvokeOnMainThread(() =>
                        //{
                        //    var confirmCopyToast = Toast.Make("Checklista jest teraz publiczna", ToastDuration.Short, 14);
                        //    confirmCopyToast.Show();
                        //});
                    }
                });

                //gdy ktos zmieni nazwe checklisty
                m_Connection.On<string>("CheckListChangeNameReceived", (message) =>
                {
                    CheckList.Name = message;
                    //MainThread.BeginInvokeOnMainThread(() =>
                    //{
                    //    var confirmCopyToast = Toast.Make("Zmieniono nazwę checklisty", ToastDuration.Short, 14);
                    //    confirmCopyToast.Show();
                    //});                    
                });

                m_Connection.On<string>("SetConnection", (message) =>
                {
                    CheckListDTO cl = JsonConvert.DeserializeObject<CheckListDTO>(message);
                    if (cl.Id == CheckListId)
                    {
                        CheckList = cl;
                        CheckList.Fields = cl.Fields.ToObservableCollection();
                    }
                    //else
                    //    MainThread.BeginInvokeOnMainThread(() => Shell.Current.CurrentPage.DisplayAlert("Uwaga", $"Id checklisty niezgadza się z oczekiwanym", "Ok"));
                });

                await m_Connection.StartAsync();
                await m_Connection.InvokeCoreAsync("SetConnection", args: new[] { CheckListId.ToString() });
            }
            catch (HubException)
            {
                await Shell.Current.CurrentPage.DisplayAlert("Błąd", $"Nie udało wykonać operacji", "Ok");
            }
            catch (Exception)
            {
                await Shell.Current.CurrentPage.DisplayAlert("Błąd", $"Nieznany błąd", "Ok");
            }
        }

        void GoBackSynch()
        {
            GoBack();
        }

        [RelayCommand]
        async Task GoBack()
        {
            await m_Connection.InvokeCoreAsync("LeaveGroup", args: new[] { TourId.ToString() });

            var navigationParameter = new Dictionary<string, object>
            {
                { "passTourId",  TourId}
            };
            await Shell.Current.GoToAsync($"Tour/CheckLists", navigationParameter);
        }

        [RelayCommand]
        async Task DeleteCheckList()
        {
            if (m_Configuration.User.Id != CheckList.UserId && !IsOrganizer)
            {
                await Shell.Current.CurrentPage.DisplayAlert("Błąd", "Nie posiadasz uprawnień do tej operacji", "Ok");
                return;
            }

            var res = await Shell.Current.CurrentPage.DisplayAlert("Uwaga", $"Czy na pewno chcesz usunąć checkliste?", "Tak", "Nie");
            if (!res)
                return;

            try
            {
                await m_Connection.InvokeCoreAsync("CheckListDelete", args: new[] { CheckListId.ToString() });
            }
            catch (HubException ex)
            {
                await Shell.Current.CurrentPage.DisplayAlert("Błąd", $"{ex.Message}", "Ok");
            }
            catch (Exception)
            {
                await Shell.Current.CurrentPage.DisplayAlert("Błąd", $"Nieznany błąd", "Ok");
            }
        }

        [RelayCommand]
        async Task ChangeVisibilityCheckList()
        {
            if (m_Configuration.User.Id != CheckList.UserId && !IsOrganizer)
            {
                await Shell.Current.CurrentPage.DisplayAlert("Błąd", "Nie posiadasz uprawnień do tej operacji", "Ok");
                return;
            }

            string str = "";
            if (CheckList.IsPublic)
                str = "prywatna";
            else
                str = "publiczna";

            var res = await Shell.Current.CurrentPage.DisplayAlert("Uwaga", $"Czy na pewno chcesz aby checklista była od teraz {str}?", "Tak", "Nie");
            if (!res)
                return;

            try
            {
                EditCheckListDTO edit = new EditCheckListDTO();
                edit.IsPublic = !CheckList.IsPublic;
                edit.Name = CheckList.Name;
                edit.Id = CheckListId;

                string json = JsonConvert.SerializeObject(edit);
                await m_Connection.InvokeCoreAsync("CheckListChangeVisibility", args: new[] { json.ToString() });
            }
            catch (HubException ex)
            {
                await Shell.Current.CurrentPage.DisplayAlert("Błąd", $"{ex.Message}", "Ok");
            }
            catch (Exception)
            {
                await Shell.Current.CurrentPage.DisplayAlert("Błąd", $"Nieznany błąd", "Ok");
            }
        }

        [RelayCommand]
        async Task ChangeNameCheckList()
        {
            if (m_Configuration.User.Id != CheckList.UserId && !IsOrganizer)
            {
                await Shell.Current.CurrentPage.DisplayAlert("Błąd", "Nie posiadasz uprawnień do tej operacji", "Ok");
                return;
            }

            var result = await Shell.Current.CurrentPage.DisplayPromptAsync("Wprowadź nową nazwę checklisty", "", "OK", "Anuluj");
            if (string.IsNullOrEmpty(result))
            {
                await Shell.Current.CurrentPage.DisplayAlert("Błąd", "Nowa nazwa nie może być pusta", "Ok");
                return;
            }

            EditCheckListDTO edit = new EditCheckListDTO();
            edit.IsPublic = CheckList.IsPublic;
            edit.Name = result.ToString().TrimStart().TrimEnd();
            edit.Id = CheckListId;

            try
            {
                string json = JsonConvert.SerializeObject(edit);
                await m_Connection.InvokeCoreAsync("CheckListChangeName", args: new[] { json.ToString() });
            }
            catch (HubException ex)
            {
                await Shell.Current.CurrentPage.DisplayAlert("Błąd", $"{ex.Message}", "Ok");
            }
            catch (Exception)
            {
                await Shell.Current.CurrentPage.DisplayAlert("Błąd", $"Nieznany błąd", "Ok");
            }
        }

        [RelayCommand]
        async Task DeleteField(CheckListFieldDTO field)
        {
            var res = await Shell.Current.CurrentPage.DisplayAlert("Uwaga", "Czy na pewno chcesz usunąć pole z checklisty?", "Tak", "Nie");
            if (!res)
                return;

            try
            {
                await m_Connection.InvokeCoreAsync("CheckListFieldDelete", args: new[] { field.Id.ToString() });
            }
            catch (HubException ex)
            {
                await Shell.Current.CurrentPage.DisplayAlert("Błąd", $"{ex.Message}", "Ok");
            }
            catch (Exception)
            {
                await Shell.Current.CurrentPage.DisplayAlert("Błąd", $"Nieznany błąd", "Ok");
            }
        }

        [RelayCommand]
        async Task AddField()
        {
            List<Tuple2String> list = new List<Tuple2String>();
            foreach (var item in CheckList.Fields)
                list.Add(new Tuple2String { Name = item.Name, Multiplicity = item.Multiplicity });

            Tuple2String result = (Tuple2String)await Shell.Current.CurrentPage.ShowPopupAsync(new AddCheckListFieldPopups(list));
            if (result is null)
                return;

            CreateCheckListFieldDTO field = new CreateCheckListFieldDTO();
            field.CheckListId = CheckListId;
            field.Name = result.Name;
            field.Multiplicity = result.Multiplicity;

            try
            {
                string json = JsonConvert.SerializeObject(field);
                await m_Connection.InvokeCoreAsync("CheckListFieldAdd", args: new[] { json.ToString() });
            }
            catch (HubException ex)
            {
                await Shell.Current.CurrentPage.DisplayAlert("Błąd", $"{ex.Message}", "Ok");
            }
            catch (Exception)
            {
                await Shell.Current.CurrentPage.DisplayAlert("Błąd", $"Nieznany błąd", "Ok");
            }
        }

        [RelayCommand]
        async Task CheckField(CheckListFieldDTO field)
        {
            EditCheckListFieldDTO newField = new EditCheckListFieldDTO();
            newField.Name = field.Name;
            newField.CheckListId = field.CheckListId;
            newField.Multiplicity = field.Multiplicity;
            newField.IsChecked = !field.IsChecked;
            newField.Id = field.Id;

            try
            {
                string json = JsonConvert.SerializeObject(newField);
                await m_Connection.InvokeCoreAsync("CheckListFieldCheck", args: new[] { json.ToString() });
            }
            catch (HubException ex)
            {
                await Shell.Current.CurrentPage.DisplayAlert("Błąd", $"{ex.Message}", "Ok");
            }
            catch (Exception)
            {
                await Shell.Current.CurrentPage.DisplayAlert("Błąd", $"Nieznany błąd", "Ok");
            }
        }
    }
}
