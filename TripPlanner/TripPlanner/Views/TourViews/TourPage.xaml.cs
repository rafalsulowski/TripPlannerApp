using Microsoft.Maui.ApplicationModel;
using System.Runtime.InteropServices;
using TripPlanner.Models.DTO.TourDTOs;
using TripPlanner.ViewModels;
using TripPlanner.ViewModels.Tour;

namespace TripPlanner.Views.TourViews;

public partial class TourPage : ContentPage
{
	public TourPage(TourViewModel tourViewModel)
	{
		InitializeComponent();
		BindingContext = tourViewModel;
	}
}