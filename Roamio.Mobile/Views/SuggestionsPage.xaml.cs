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
		var experienceSuggestions = await _googleMapsService.GetActivitySuggestionsAsync(currentTrip.Destination, currentTrip.UserPreferences.ActivityPreferences);
        SuggestedRestaurantsCollection.ItemsSource = restaurantSuggestions;
        SuggestedExperiencesCollection.ItemsSource = experienceSuggestions;

		await Task.CompletedTask;
    }

    private void OnActivitiesStepperValueChanged(object sender, ValueChangedEventArgs e)
	{
		ActivitiesLabel.Text = ((int)e.NewValue).ToString();
	}

	private async void OnConfirmClicked(object sender, EventArgs e)
	{
		var selectedRestaurants = SuggestedRestaurantsCollection.SelectedItems?.Cast<SuggestionItem>().ToList() ?? new List<SuggestionItem>();
        var selectedExperiences = SuggestedExperiencesCollection.SelectedItems?.Cast<SuggestionItem>().ToList() ?? new List<SuggestionItem>();

        var currentTrip = TripDataStore.CurrentTrip;
        if (currentTrip == null)
        {
            await DisplayAlert("Error", "No trip data available.", "OK");
            return;
        }

		currentTrip.DailyPlans = new List<string>();
		foreach (var r in selectedRestaurants)
			currentTrip.DailyPlans.Add(r.Name);
        foreach (var exp in selectedExperiences)
            currentTrip.DailyPlans.Add(exp.Name);

        var timeSlot = $"{StartTimePicker.Time} to {EndTimePicker.Time}";
        int activitiesPerDay = (int)ActivitiesStepper.Value;

        string message = $"Destination: {currentTrip.Destination}\n" +
                             $"Dates: {currentTrip.StartDate} to {currentTrip.EndDate}\n" +
                             $"Time Slot: {timeSlot}\n" +
                             $"Activities per day: {activitiesPerDay}\n" +
                             $"Selected Restaurants: {string.Join(", ", selectedRestaurants.Select(s => s.Name))}\n" +
                             $"Selected Activities: {string.Join(", ", selectedExperiences.Select(s => s.Name))}";

        await DisplayAlert("Confirm Selections", message, "OK");
    }    
}