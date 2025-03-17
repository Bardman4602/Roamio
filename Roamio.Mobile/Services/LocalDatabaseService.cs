using SQLite;
using System.IO;
using System.Threading.Tasks;
using Roamio.Mobile.Models;
using System.Text.Json;

namespace Roamio.Mobile.Services
{
    class LocalDatabaseService
    {
        private readonly SQLiteAsyncConnection _database;

        public LocalDatabaseService(string dbPath)
        {
            _database = new SQLiteAsyncConnection(dbPath);
            _database.CreateTableAsync<Trip>().Wait();
        }

        public async Task SaveTripAsync(Trip trip)
        {
            trip.UserPreferencesJson = JsonSerializer.Serialize(trip.UserPreferences);
            trip.GoogleMapsDataJson = JsonSerializer.Serialize(trip.GoogleMapsData);
            trip.DailyPlansJson = JsonSerializer.Serialize(trip.DailyPlans);
            trip.RestaurantSelectionsJson = JsonSerializer.Serialize(trip.RestaurantSelections);
            trip.ActivitySelectionsJson = JsonSerializer.Serialize(trip.ActivitySelections);
            trip.DayPlansJson = JsonSerializer.Serialize(trip.DayPlans);

            await _database.InsertOrReplaceAsync(trip);
        }

        public async Task<Trip> LoadTripAsync(string tripId)
        {            
            var trip = await _database.Table<Trip>()
                .Where(t => t.Id == tripId)
                .FirstOrDefaultAsync();

            if (trip == null)
                return null;

            if (!string.IsNullOrEmpty(trip.UserPreferencesJson))
                trip.UserPreferences = JsonSerializer.Deserialize<UserPreferences>(trip.UserPreferencesJson);

            if (!string.IsNullOrEmpty(trip.GoogleMapsDataJson))
                trip.GoogleMapsData = JsonSerializer.Deserialize<Dictionary<string, string>>(trip.GoogleMapsDataJson);

            if (!string.IsNullOrEmpty(trip.DailyPlansJson))
                trip.DailyPlans = JsonSerializer.Deserialize<List<string>>(trip.DailyPlansJson);

            if (!string.IsNullOrEmpty(trip.RestaurantSelectionsJson))
                trip.RestaurantSelections = JsonSerializer.Deserialize<List<string>>(trip.RestaurantSelectionsJson);

            if (!string.IsNullOrEmpty(trip.ActivitySelectionsJson))
                trip.ActivitySelections = JsonSerializer.Deserialize<List<string>>(trip.ActivitySelectionsJson);

            if (!string.IsNullOrEmpty(trip.DayPlansJson))
                trip.DayPlans = JsonSerializer.Deserialize<List<DayPlan>>(trip.DayPlansJson);

            return trip;
        }
    }
}
