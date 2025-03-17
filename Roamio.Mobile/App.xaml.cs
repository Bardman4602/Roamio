using Roamio.Mobile.Services;
using Roamio.Mobile.Models;

namespace Roamio.Mobile
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            MainPage = new AppShell();
            LoadSavedTrip();
        }

        private async void LoadSavedTrip()
        {
            try
            {
                var dbService = MauiProgram.Services.GetRequiredService<LocalDatabaseService>();                
                var savedTrip = await dbService.LoadTripAsync("");
                if (savedTrip != null)
                {
                    TripDataStore.CurrentTrip = savedTrip;
                }
            }
            catch (Exception ex)
            {                
                System.Diagnostics.Debug.WriteLine($"Error loading saved trip: {ex.Message}");
            }
        }
    }
}
