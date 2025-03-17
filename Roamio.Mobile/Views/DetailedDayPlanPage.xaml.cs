using System.Linq;
using System.Threading.Tasks;
using Roamio.Mobile.Models;

namespace Roamio.Mobile.Views;

public partial class DetailedDayPlanPage : ContentPage
{
	private DayPlan _dayPlan;

	public DetailedDayPlanPage(DayPlan dayPlan)
	{
		InitializeComponent();
        _dayPlan = dayPlan;
    }

    protected override void OnAppearing()
	{
        base.OnAppearing();

        if (_dayPlan != null)
        {
            DayPlanTitleLabel.Text = $"Day {_dayPlan.DayNumber} - {_dayPlan.Date:yyyy-MM-dd}";
            ScheduleCollection.ItemsSource = _dayPlan.Schedule;
        }
    }

    private async void OnGetDirectionsClicked(object sender, EventArgs e)
    {
        string travelMode = TransportModePicker.SelectedItem as string ?? "Public Transit";

        if (_dayPlan == null || _dayPlan.Schedule == null || _dayPlan.Schedule.Count == 0)
        {
            await DisplayAlert("Error", "No schedule available.", "OK");
            return;
        }

        if (_dayPlan.Schedule.Count == 1)
        {
            if (!await CheckLocationPermissionAsync())
            {
                await DisplayAlert("Permission Denied", "Location permission is required.", "OK");
                return;
            }

            var location = await Geolocation.GetLocationAsync(new GeolocationRequest
            {
                DesiredAccuracy = GeolocationAccuracy.High
            });

            if (location == null)
            {
                await DisplayAlert("Error", "Unable to determine current location.", "OK");
                return;
            }

            string originQuery = $"{location.Latitude},{location.Longitude}";
            string destinationQuery = _dayPlan.Schedule[0].LocationQuery ?? _dayPlan.Schedule[0].Name;

            string url = $"https://www.google.com/maps/dir/?api=1&origin={Uri.EscapeDataString(originQuery)}&destination={Uri.EscapeDataString(destinationQuery)}&travelmode={travelMode}";
            DebugUrlLabel.Text = url;
            DebugUrlLabel.IsVisible = true;

            try
            {
                await Launcher.OpenAsync(url);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Unable to open directions: {ex.Message}", "OK");
            }
        }
        else
        {
            string originQuery = _dayPlan.Schedule.First().LocationQuery ?? _dayPlan.Schedule.First().Name;
            string destinationQuery = _dayPlan.Schedule.Last().LocationQuery ?? _dayPlan.Schedule.Last().Name;

            var waypoints = _dayPlan.Schedule.Skip(1).Take(_dayPlan.Schedule.Count - 2)
                               .Select(item => item.LocationQuery ?? item.Name)
                               .Where(q => !string.IsNullOrWhiteSpace(q))
                               .ToList();

            string baseUrl = "https://www.google.com/maps/dir/?api=1";
            string originParam = $"origin={Uri.EscapeDataString(originQuery)}";
            string destParam = $"destination={Uri.EscapeDataString(destinationQuery)}";
            string travelModeParam = $"travelmode={travelMode}";
            string waypointsParam = "";

            if (waypoints.Any())
            {
                waypointsParam = $"waypoints={string.Join("|", waypoints.Select(Uri.EscapeDataString))}";
            }

            string finalUrl = $"{baseUrl}&{originParam}&{destParam}&{travelModeParam}";
            if (!string.IsNullOrEmpty(waypointsParam))
            {
                finalUrl += $"&{waypointsParam}";
            }

            DebugUrlLabel.Text = finalUrl;
            DebugUrlLabel.IsVisible = true;

            try
            {
                await Launcher.OpenAsync(finalUrl);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Unable to open directions: {ex.Message}", "OK");
            }
        } 
    }

    private async Task<bool> CheckLocationPermissionAsync()
    {
        var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
        if (status != PermissionStatus.Granted)
        {
            status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
        }
        return status == PermissionStatus.Granted;
    }
}