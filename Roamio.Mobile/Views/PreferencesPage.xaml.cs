using System.Collections.Generic;
using System.Linq;
using Roamio.Mobile.Models;

namespace Roamio.Mobile.Views;

public partial class PreferencesPage : ContentPage
{
	private int _mealsPerDay = 0;

    public PreferencesPage()
	{
		InitializeComponent();
        MealsStepper.ValueChanged += OnMealsStepperValueChanged;

        if (TripDataStore.CurrentTrip != null && !string.IsNullOrEmpty(TripDataStore.CurrentTrip.Destination))
        {
            Title = $"Destination: {TripDataStore.CurrentTrip.Destination}";
        }
    }    

	private void OnMealsStepperValueChanged(object sender, ValueChangedEventArgs e)
	{
        _mealsPerDay = (int)e.NewValue;
        MealsLabel.Text = _mealsPerDay.ToString();
    }

	private async void OnSavePreferencesClicked(object sender, EventArgs e)
	{
        // rewrite these like the activity level radio buttons
        var foodPrefs = new List<string>();
		if (IndianCheck.IsChecked) foodPrefs.Add("Indian");
        if (JapaneseCheck.IsChecked) foodPrefs.Add("Japanese");
        if (AmericanCheck.IsChecked) foodPrefs.Add("American");
        if (ItalianCheck.IsChecked) foodPrefs.Add("Italian");
        if (BritishCheck.IsChecked) foodPrefs.Add("British");
        if (FrenchCheck.IsChecked) foodPrefs.Add("French");
        if (ThaiCheck.IsChecked) foodPrefs.Add("Thai");
        if (ChineseCheck.IsChecked) foodPrefs.Add("Chinese");
        if (GreekCheck.IsChecked) foodPrefs.Add("Greek");

        var experiencePrefs = new List<string>();
        if (MuseumCheck.IsChecked) experiencePrefs.Add("Museum");
        if (ZooCheck.IsChecked) experiencePrefs.Add("Zoo");
        if (SpaCheck.IsChecked) experiencePrefs.Add("Spa and wellness");
        if (BeachCheck.IsChecked) experiencePrefs.Add("Beach");
        if (CastleCheck.IsChecked) experiencePrefs.Add("Castles and fortresses");
        if (ShoppingCheck.IsChecked) experiencePrefs.Add("Shopping");
        if (OutdoorCheck.IsChecked) experiencePrefs.Add("Hiking and outdoor");
        if (ExtremeCheck.IsChecked) experiencePrefs.Add("Extreme sports (ziplining, bungee jump etc.)");
        if (ExplorationCheck.IsChecked) experiencePrefs.Add("Exploration");

        string activityLevel = null;
        if (ActivityLevelContainer != null)
        {
            foreach (var child in ActivityLevelContainer.Children)
            {
                if (child is RadioButton rb && rb.IsChecked)
                {
                    activityLevel = rb.Value?.ToString();
                    break;
                }
            }
        }

        var currentTrip = TripDataStore.CurrentTrip;
        if (currentTrip == null)
        {
            currentTrip = new Trip();
            TripDataStore.CurrentTrip = currentTrip;
        }
        if (currentTrip.UserPreferences == null)
        {
            currentTrip.UserPreferences = new UserPreferences();
        }

        if (string.IsNullOrEmpty(currentTrip.UserPreferences.Id))
        {
            currentTrip.UserPreferences.Id = Guid.NewGuid().ToString();
        }

        currentTrip.UserPreferences.UserId = currentTrip.UserId;
        currentTrip.UserPreferences.MealsPerDay = _mealsPerDay;
        currentTrip.UserPreferences.FoodPreferences = foodPrefs;
        currentTrip.UserPreferences.ActivityPreferences = experiencePrefs;
        currentTrip.UserPreferences.EnergyLevel = MapActivityLevelToInt(activityLevel);

        // Delete this after testing
        var nessage = 
                $"Destination: {currentTrip.Destination}\n" +
                $"Start Date: {currentTrip.StartDate}\n" +
                $"End Date: {currentTrip.EndDate}\n" +
                $"Meals per day: {_mealsPerDay}\n" +
                $"Activity level: {activityLevel}\n" +
                $"Food: {string.Join(", ", foodPrefs)}\n" +
                $"Experiences: {string.Join(", ", experiencePrefs)}";
        await DisplayAlert("Confirm Selections", nessage, "OK");


        // Save destination and user prefs
        // Navigate to SuggestionsPage
        // On SuggestionsPage, get suggestions from GoogleMapsAPI based on preferences and destination
        // Selected suggestions will be added to currentTrip.DailyPlans
    }

    private int MapActivityLevelToInt(string activityLevel)
    {
        return activityLevel switch
        {
            "Very low" => 1,
            "Low" => 2,
            "Medium" => 3,
            "High" => 4,
            "Very high" => 5,
            _ => 0
        };
    }
}