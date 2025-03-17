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
            DayPlanTitleLabel.Text = $"Day {_dayPlan.DayNumber} - {_dayPlan.Date:dd/MM/yyyy}";
            ScheduleCollection.ItemsSource = _dayPlan.Schedule;
        }
    }

    private async void OnGetDirectionsForItemClicked(object sender, EventArgs e)
    {
        if (sender is Button btn && btn.CommandParameter is ScheduleItem item)
        {
            string travelMode = TransportModePicker.SelectedItem as string ?? "public transit";

            if (!await CheckLocationPermissionAsync())
            {
                await DisplayAlert("Location Permission Required", "Please enable location permissions to use this feature.", "OK");
                return;
            }

            var request = new GeolocationRequest(GeolocationAccuracy.High, TimeSpan.FromSeconds(10));
            var location = await Geolocation.GetLocationAsync(request);

            try
            {
                if (location == null)
                {
                    await DisplayAlert("Error", "Unable to determine current location.", "OK");
                    return;
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Location error: {ex.Message}", "OK");
                return;
            }

            string originQuery = $"{location.Latitude},{location.Longitude}";
            string destinationQuery = item.LocationQuery ?? item.Name;

            string url = $"https://www.google.com/maps/dir/?api=1&origin={Uri.EscapeDataString(originQuery)}&destination={Uri.EscapeDataString(destinationQuery)}&travelmode={travelMode}";

            DebugUrlLabel.Text = url;
            DebugUrlLabel.IsVisible = true;

            try
            {
                await Launcher.OpenAsync(new Uri(url));
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