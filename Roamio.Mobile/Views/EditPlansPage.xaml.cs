using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Roamio.Mobile.Models;     
using Roamio.Mobile.Services;

namespace Roamio.Mobile.Views;

public partial class EditPlansPage : ContentPage
{
    private readonly IGoogleMapsService _googleMapsService;
    private readonly IApiService _apiService;

    public EditPlansPage()
            : this(MauiProgram.Services.GetRequiredService<IGoogleMapsService>(),
                   MauiProgram.Services.GetRequiredService<IApiService>())
    {
    }

    public EditPlansPage(IGoogleMapsService googleMapsService, IApiService apiService)
	{
		InitializeComponent();
        _googleMapsService = googleMapsService;
        _apiService = apiService;

        var currentTrip = TripDataStore.CurrentTrip;
        if (currentTrip != null)
        {
            Title = $"Edit plans for {currentTrip.Destination}";
        }
        else
        {
            Title = "Edit Plans";
        }

        RestaurantsStepper.ValueChanged += OnRestaurantsStepperValueChanged;
        ActivitiesStepper.ValueChanged += OnActivitiesStepperValueChanged;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        await LoadAndPreselectSuggestionsAsync();
    }

    private async Task LoadAndPreselectSuggestionsAsync()
    {
        var currentTrip = TripDataStore.CurrentTrip;
        if (currentTrip == null)
        {
            await DisplayAlert("Error", "No trip data available.", "OK");
            return;
        }

        var restaurantSuggestions = await _googleMapsService.GetRestaurantSuggestionsAsync(currentTrip.Destination, currentTrip.UserPreferences?.FoodPreferences ?? new List<string>());
        var activitySuggestions = await _googleMapsService.GetActivitySuggestionsAsync(currentTrip.Destination, currentTrip.UserPreferences?.ActivityPreferences ?? new List<string>());

        SuggestedRestaurantsCollection.ItemsSource = restaurantSuggestions;
        SuggestedExperiencesCollection.ItemsSource = activitySuggestions;

        // Load the current selections
        if (currentTrip.RestaurantSelections != null)
        {
            foreach (var suggestion in restaurantSuggestions)
            {
                if (currentTrip.RestaurantSelections.Contains(suggestion.Name))
                {
                    if (!SuggestedRestaurantsCollection.SelectedItems.Contains(suggestion))
                    {
                        SuggestedRestaurantsCollection.SelectedItems.Add(suggestion);
                    }
                }
            }
        }

        if (currentTrip.ActivitySelections != null)
        {
            foreach (var suggestion in activitySuggestions)
            {
                if (currentTrip.ActivitySelections.Contains(suggestion.Name))
                {
                    if (!SuggestedExperiencesCollection.SelectedItems.Contains(suggestion))
                    {
                        SuggestedExperiencesCollection.SelectedItems.Add(suggestion);
                    }
                }
            }
        }

        await Task.CompletedTask;
    }

    private void OnRestaurantsStepperValueChanged(object sender, ValueChangedEventArgs e)
    {
        RestaurantsLabel.Text = ((int)e.NewValue).ToString();
    }

    private void OnActivitiesStepperValueChanged(object sender, ValueChangedEventArgs e)
    {
        ActivitiesLabel.Text = ((int)e.NewValue).ToString();
    }

    private async void OnConfirmClicked(object sender, EventArgs e)
    {        
        var selectedRestaurants = SuggestedRestaurantsCollection.SelectedItems?.Cast<SuggestionItem>().ToList() ?? new List<SuggestionItem>();
        var selectedActivities = SuggestedExperiencesCollection.SelectedItems?.Cast<SuggestionItem>().ToList() ?? new List<SuggestionItem>();

        var currentTrip = TripDataStore.CurrentTrip;
        if (currentTrip == null)
        {
            await DisplayAlert("Error", "No trip data available.", "OK");
            return;
        }
        
        currentTrip.RestaurantSelections = selectedRestaurants.Select(r => r.Name).Distinct().ToList();
        currentTrip.ActivitySelections = selectedActivities.Select(a => a.Name).Distinct().ToList();

        DayPlanGenerator.GenerateDayPlans(
            currentTrip,
            currentTrip.RestaurantSelections,
            currentTrip.ActivitySelections,
            StartTimePicker.Time,
            EndTimePicker.Time
            );

        await DisplayAlert("Plan Updated", "Your selections have been updated.", "OK");
        
        await Navigation.PopAsync();
    }
}