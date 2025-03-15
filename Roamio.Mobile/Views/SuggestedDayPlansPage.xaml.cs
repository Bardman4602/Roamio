using System.Collections.Generic;
using System.Linq;
using Roamio.Mobile.Models;

namespace Roamio.Mobile.Views;

public partial class SuggestedDayPlansPage : ContentPage
{
	public SuggestedDayPlansPage()
	{
		InitializeComponent();
	}

    protected override void OnAppearing()
    {
        base.OnAppearing();

        var currentTrip = TripDataStore.CurrentTrip;

        if (currentTrip != null)
        {
            if (DateTime.TryParse(currentTrip.StartDate, out var start) &&
            DateTime.TryParse(currentTrip.EndDate, out var end))
            {
                int dayCount = (end - start).Days + 1;
                Title = $"Suggested plans for {dayCount} days in {currentTrip.Destination}";
            }
            else
            {
                Title = "Suggested day plans";
            }
        }

        if (currentTrip?.DayPlans != null && currentTrip.DayPlans.Any())
        {
            DayPlansCollection.ItemsSource = currentTrip.DayPlans;
        }
        else
        {
            DisplayAlert("Error", "No dayplans found", "OK");
        }        
    }    

    private async void OnChangePlanClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new EditPlansPage());
    }

    private async void OnConfirmPlanClicked(object sender, EventArgs e)
    {
        await DisplayAlert("Plan Confirmed", "Your day plan has been confirmed!", "OK");

        // Save evetything to the database
        // Go to DisplayDayPlanPage
    }    
}