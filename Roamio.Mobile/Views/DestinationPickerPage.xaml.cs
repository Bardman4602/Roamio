using Roamio.Mobile.Services;
using Roamio.Mobile.Models;

namespace Roamio.Mobile.Views;

public partial class DestinationPickerPage : ContentPage
{
    private readonly IGoogleMapsService _googleMapsService;
    private readonly IApiService _apiService;

    public DestinationPickerPage()
        : this(MauiProgram.Services.GetRequiredService<IGoogleMapsService>(),
               MauiProgram.Services.GetRequiredService<IApiService>())
        
    {        
    }

    public DestinationPickerPage(IGoogleMapsService googleMapsService, IApiService apiService)
    {
        InitializeComponent();
        _googleMapsService = googleMapsService;
        _apiService = apiService;
    }

    private async void OnNextClicked(object sender, EventArgs e)
    {
        var destination = DestinationEntry.Text;
        var startDate = StartDatePicker.Date;
        var endDate = EndDatePicker.Date;

        if (string.IsNullOrWhiteSpace(destination))
        {
            await DisplayAlert("Missing info", "Please enter a destination", "OK");
            return;
        }

        if (startDate > endDate)
        {
            await DisplayAlert("Invalid Dates", "Start date cannot be after end date.", "OK");
            return;
        }

        var geocodeResponse = await _googleMapsService.GetGeocodeAsync(destination);
        if (geocodeResponse == null || geocodeResponse.status != "OK" || geocodeResponse.results.Length == 0)
        {
            await DisplayAlert("Error", "Failed to retrieve location data from Google Maps.", "OK");
            return;
        }

        var result = geocodeResponse.results[0];
        var googleMapsData = new Dictionary<string, string>
        {
            { "FormattedAddress", result.formatted_address },
            { "Latitude", result.geometry.location.lat.ToString() },
            { "Longitude", result.geometry.location.lng.ToString() }
        };

        var newTrip = new Trip
        {      
            Id = "test1",
            UserId = "dummu-user",
            Destination = destination,
            StartDate = startDate.ToString("dd/MM/yyyy"),
            EndDate = endDate.ToString("dd/MM/yyyy"),
            GoogleMapsData = googleMapsData,
            DailyPlans = new List<string>()
        };

        var createdTrip = await _apiService.CreateTripAsync(newTrip);
        if (createdTrip != null)
        {
            DisplayAlert("Success", "Trip created successfully!", "OK");
            // Navigate to TripPreferences page
        }
        else
        {
            await DisplayAlert("Error", "Trip creation failed. Please try again.", "OK");
        }
    }
}