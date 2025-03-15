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

        var restaurantNames = selectedRestaurants.Select(r => r.Name).Distinct().ToList();
        var activityNames = selectedActivities.Select(a => a.Name).Distinct().ToList();        

        if (restaurantNames.Count <= selectedRestaurants.Count * 2 || activityNames.Count <= selectedActivities.Count * 2)
        {
            bool proceed = await DisplayAlert("Warning", "Not enough selections to fill all days within the timeslot. Your days will have fewer activities. More can be added later. Proceed anyway?", "Yes", "No");
            if (!proceed)
            {                
                return;
            }
        }

        GenerateDayPlans(currentTrip, restaurantNames, activityNames);

        await DisplayAlert("Plan Created", "Plan created!", "OK");

        await Navigation.PushAsync(new SuggestedDayPlansPage());
    }

    private async Task GenerateDayPlans(Trip currentTrip, List<string> restaurants, List<string> activities)
    {
        currentTrip.DayPlans.Clear();

        if (!DateTime.TryParse(currentTrip.StartDate, out var startDate) || !DateTime.TryParse(currentTrip.EndDate, out var endDate))
        {
            return;
        }

        var startTime = StartTimePicker.Time;
        var endTime = EndTimePicker.Time;
        int totalDays = (endDate - startDate).Days + 1;
        if (totalDays <= 0) return;
        int mealsPerDay = currentTrip.UserPreferences.MealsPerDay;
        //int activitiesPerDay = currentTrip.UserPreferences.ActivityPreferences.Count > 0
        //    ? currentTrip.UserPreferences.EnergyLevel
        //    : currentTrip.UserPreferences.EnergyLevel;

        int totalRestaurants = restaurants.Count;
        int restaurantsPerDay = totalRestaurants / totalDays;
        int remainderRest = totalRestaurants % totalDays;

        int totalActivities = activities.Count;
        int activitiesPerDay = totalActivities / totalDays;
        int remainderAct = totalActivities % totalDays;

        for (int d = 0; d < totalDays; d++)
        {
            var date = startDate.AddDays(d);

            var dayPlan = new DayPlan
            {
                DayNumber = d + 1,
                Date = date,
                Schedule = new List<ScheduleItem>()
            };

            int dayRestCount = restaurantsPerDay + (d < remainderRest ? 1 : 0);
            int dayActCount = activitiesPerDay + (d < remainderAct ? 1 : 0);

            var dayRestaurants = restaurants.Take(dayRestCount).ToList();
            restaurants.RemoveRange(0, Math.Min(dayRestCount, restaurants.Count));

            var dayActivities = activities.Take(dayActCount).ToList();
            activities.RemoveRange(0, Math.Min(dayActCount, activities.Count));

            TimeSpan currentTime = startTime;
            int maxCount = Math.Max(dayRestCount, dayActCount);
            for (int i = 0; i < maxCount; i++)
            {
                if (i < dayActivities.Count)
                {
                    dayPlan.Schedule.Add(new ScheduleItem
                    {
                        Time = FormatTime(currentTime),
                        Name = dayActivities[i],
                        Type = "Activity"
                    });
                    currentTime = currentTime.Add(TimeSpan.FromHours(2)); // assuming 2 hours of activity - adjust as needed
                    if (currentTime >= endTime) break;
                }

                if (i < dayRestaurants.Count)
                {
                    dayPlan.Schedule.Add(new ScheduleItem
                    {
                        Time = FormatTime(currentTime),
                        Name = dayRestaurants[i],
                        Type = "Restaurant"
                    });
                    currentTime = currentTime.Add(TimeSpan.FromHours(2)); // assuming 2 hours for a meal - adjust as needed
                    if (currentTime >= endTime) break;
                }
            }
            currentTrip.DayPlans.Add(dayPlan);

            //int activitiesUsed = 0;
            //int mealsUsed = 0;
            //while (currentTime < endTime)
            //{
            //    if (activitiesUsed < activitiesPerDay && activities.Any())
            //    {
            //        var activity = activities.First();
            //        activities.RemoveAt(0);
            //        dayPlan.Schedule.Add(new ScheduleItem
            //        {
            //            Time = FormatTime(currentTime),
            //            Name = activity,
            //            Type = "Activity"
            //        });
            //        activitiesUsed++;
            //        currentTime = currentTime.Add(TimeSpan.FromHours(2)); // assuming 2 hours of activity - adjust as needed
            //    }
            //    else
            //    {
            //        break;
            //    }

            //    if (mealsUsed < mealsPerDay && restaurants.Any())
            //    {
            //        var restaurant = restaurants.First();
            //        restaurants.RemoveAt(0);
            //        dayPlan.Schedule.Add(new ScheduleItem
            //        {
            //            Time = FormatTime(currentTime),
            //            Name = restaurant,
            //            Type = "Restaurant"
            //        });
            //        mealsUsed++;
            //        currentTime = currentTime.Add(TimeSpan.FromHours(2)); // assuming 2 hours for a meal - adjust as needed
            //    }
            //    else
            //    {
            //        // no more meals or time left
            //        // maybe add option to expand the day? - possible future feature                    
            //    }
            //}
            //currentTrip.DayPlans.Add(dayPlan);
        }
    }

    private string FormatTime(TimeSpan time)
    {
        string formatted = time.ToString("hh\\:mm");
        return (formatted == "00:00") ? "" : formatted;
    }
}