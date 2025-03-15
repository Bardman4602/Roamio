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
        await Navigation.PopAsync(); // Go back to the previous page - will be changed to EditPlanPage

        // Go to EditPlanPage - Not implemented yet
    }

    private async void OnConfirmPlanClicked(object sender, EventArgs e)
    {
        await DisplayAlert("Plan Confirmed", "Your day plan has been confirmed!", "OK");

        // Save evetything to the database
        // Go to DisplayDayPlanPage
    }
}