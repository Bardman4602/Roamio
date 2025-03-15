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
		var selectedRestaurants = SuggestedRestaurantsCollection.SelectedItems?.Cast<SuggestionItem>().ToList() ?? new List<SuggestionItem>();
        var selectedActivities = SuggestedExperiencesCollection.SelectedItems?.Cast<SuggestionItem>().ToList() ?? new List<SuggestionItem>();

        var currentTrip = TripDataStore.CurrentTrip;
        if (currentTrip == null)
        {
            await DisplayAlert("Error", "No trip data available.", "OK");
            return;
        }

		//currentTrip.DailyPlans = new List<string>();
		//foreach (var r in selectedRestaurants)
		//	currentTrip.DailyPlans.Add(r.Name);
  //      foreach (var exp in selectedExperiences)
  //          currentTrip.DailyPlans.Add(exp.Name);

  //      var timeSlot = $"{StartTimePicker.Time} to {EndTimePicker.Time}";
  //      int activitiesPerDay = (int)ActivitiesStepper.Value;

  //      string message = $"Destination: {currentTrip.Destination}\n" +
  //                           $"Dates: {currentTrip.StartDate} to {currentTrip.EndDate}\n" +
  //                           $"Time Slot: {timeSlot}\n" +
  //                           $"Activities per day: {activitiesPerDay}\n" +
  //                           $"Selected Restaurants: {string.Join(", ", selectedRestaurants.Select(s => s.Name))}\n" +
  //                           $"Selected Activities: {string.Join(", ", selectedExperiences.Select(s => s.Name))}";

  //      await DisplayAlert("Confirm Selections", message, "OK");

		var restaurantNames = selectedRestaurants.Select(r => r.Name).Distinct().ToList();
        var activityNames = selectedActivities.Select(r => r.Name).Distinct().ToList();

		GenerateDayPlans(currentTrip, restaurantNames, activityNames);

        await DisplayAlert("Plan Created", "A multi-day plan has been generated. Next, you'll see the day-by-day plan.", "OK");

        await Navigation.PushAsync(new SuggestedDayPlansPage());
    }

    private void GenerateDayPlans(Trip currentTrip, List<string> restaurants, List<string> activities)
    {
        currentTrip.DayPlans.Clear();

		if (!DateTime.TryParse(currentTrip.StartDate, out var startDate) || !DateTime.TryParse(currentTrip.EndDate, out var endDate))
		{
            return;
        }

		var startTime = StartTimePicker.Time;
		var endTime = EndTimePicker.Time;
		int totalDays = (endDate - startDate).Days + 1;
		int mealsPerDay = currentTrip.UserPreferences.MealsPerDay;
		int activitiesPerDay = currentTrip.UserPreferences.ActivityPreferences.Count > 0
			? currentTrip.UserPreferences.EnergyLevel
			: currentTrip.UserPreferences.EnergyLevel;

		for (int d = 0; d < totalDays; d++)
		{
			var date = startDate.AddDays(d);

			var dayPlan = new DayPlan
			{
				DayNumber = d + 1,
				Date = date,
				Schedule = new List<ScheduleItem>()
			};

            TimeSpan currentTime = startTime;

			int activitiesUsed = 0;
			int mealsUsed = 0;
            while (currentTime < endTime)
			{
				if (activitiesUsed < activitiesPerDay && activities.Any())
				{
					var activity = activities.First();
					activities.RemoveAt(0);
					dayPlan.Schedule.Add(new ScheduleItem
					{
						Time = currentTime.ToString(@"hh\:mm"),
						Name = activity,
						Type = "Activity"
					});
					activitiesUsed++;
					currentTime = currentTime.Add(TimeSpan.FromHours(2)); // assuming 2 hours of activity - adjust as needed
                }
				else
				{
					break;
                }

				if (mealsUsed < mealsPerDay && restaurants.Any())
				{
					var restaurant = restaurants.First();
					restaurants.RemoveAt(0);
					dayPlan.Schedule.Add(new ScheduleItem
					{
						Time = currentTime.ToString(@"hh\:mm"),
						Name = restaurant,
						Type = "Restaurant"
					});
                    mealsUsed++;
                    currentTime = currentTime.Add(TimeSpan.FromHours(2)); // assuming 2 hours for a meal - adjust as needed
                }
				else
				{
                    // no more meals or time left
                    // maybe add option to expand the day? - possible future feature
                }
            }
			currentTrip.DayPlans.Add(dayPlan);
        }
    }
}