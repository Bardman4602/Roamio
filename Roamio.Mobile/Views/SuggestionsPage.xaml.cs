using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Linq;
using Roamio.Mobile.Models;
using Roamio.Mobile.Services;

namespace Roamio.Mobile.Views;

public partial class SuggestionsPage : ContentPage
{
    private readonly IGoogleMapsService _googleMapsService;
    private readonly IApiService _apiService;

    public SuggestionsPage()
        : this(MauiProgram.Services.GetRequiredService<IGoogleMapsService>(),
                   MauiProgram.Services.GetRequiredService<IApiService>())
    {
    }

    public SuggestionsPage(IGoogleMapsService googleMapsService, IApiService apiService)
    {
        InitializeComponent();
        _googleMapsService = googleMapsService;
        _apiService = apiService;

        var currentTrip = TripDataStore.CurrentTrip;
        if (currentTrip != null)
        {
            if (DateTime.TryParse(currentTrip.StartDate, out var start) &&
                DateTime.TryParse(currentTrip.EndDate, out var end))
            {
                int dayCount = (end - start).Days + 1;
                Title = $"Suggestions for {dayCount} days in {currentTrip.Destination}";
            }
            else
            {
                Title = currentTrip.Destination;
            }
        }
        else
        {
            Title = "Suggestions";
        }

        ActivitiesStepper.ValueChanged += OnActivitiesStepperValueChanged;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await FetchSuggestionsAsync();
    }

    private async Task FetchSuggestionsAsync()
    {
        var currentTrip = TripDataStore.CurrentTrip;
        if (currentTrip == null)
        {
            await DisplayAlert("Error", "No trip data found", "OK");
            return;
        }

        var restaurantSuggestions = await _googleMapsService.GetRestaurantSuggestionsAsync(currentTrip.Destination, currentTrip.UserPreferences.FoodPreferences);
        var activitySuggestions = await _googleMapsService.GetActivitySuggestionsAsync(currentTrip.Destination, currentTrip.UserPreferences.ActivityPreferences);
        SuggestedRestaurantsCollection.ItemsSource = restaurantSuggestions;
        SuggestedExperiencesCollection.ItemsSource = activitySuggestions;

        await Task.CompletedTask;
    }

    private void OnActivitiesStepperValueChanged(object sender, ValueChangedEventArgs e)
    {
        ActivitiesLabel.Text = ((int)e.NewValue).ToString();
    }

    private async void OnConfirmClicked(object sender, EventArgs e)
    {
        var selectedRestaurants = SuggestedRestaurantsCollection.SelectedItems?
            .Cast<SuggestionItem>()
            .Select(r => r.Name)
            .Distinct()
            .ToList() ?? new List<string>();

        var selectedActivities = SuggestedExperiencesCollection.SelectedItems?
            .Cast<SuggestionItem>()
            .Select(a => a.Name)
            .Distinct()
            .ToList() ?? new List<string>();

        var currentTrip = TripDataStore.CurrentTrip;
        if (currentTrip == null)
        {
            await DisplayAlert("Error", "No trip data available.", "OK");
            return;
        }

        currentTrip.RestaurantSelections = selectedRestaurants;
        currentTrip.ActivitySelections = selectedActivities;

        double totalHours = (EndTimePicker.Time - StartTimePicker.Time).TotalHours;
        if (totalHours < 0) totalHours = 0;
        double itemsPerDay = Math.Floor(totalHours / 2.0); // assumed time per item.
        if (!DateTime.TryParse(currentTrip.StartDate, out var startDate) || !DateTime.TryParse(currentTrip.EndDate, out var endDate))
        {
            await DisplayAlert("Error", "Invalid dates", "OK");
            return;
        }
        int totalDays = (endDate - startDate).Days + 1;
        if (totalDays < 1) totalDays = 1;
        double maxPossibleItems = itemsPerDay * totalDays;
        int userSelectedCount = selectedRestaurants.Count + selectedActivities.Count;

        if (userSelectedCount < maxPossibleItems)
        {
            bool proceed = await DisplayAlert("Warning",
                "Not enough selections to fill all days within the timeslot. Your days will have fewer items.\n " +
                "This can be changed later.\n" +
                "Proceed anyway?",
                "Yes", "No");
            if (!proceed)
            {
                return;
            }
        }

        DayPlanGenerator.GenerateDayPlans(
            currentTrip,
            currentTrip.RestaurantSelections,
            currentTrip.ActivitySelections,
            StartTimePicker.Time,
            EndTimePicker.Time
        );

        var dbService = MauiProgram.Services.GetRequiredService<LocalDatabaseService>();
        await dbService.SaveTripAsync(currentTrip);

        await DisplayAlert("Plan Created", "Your plan has been created.", "OK");

        await Navigation.PushAsync(new SuggestedDayPlansPage());
    }    
}