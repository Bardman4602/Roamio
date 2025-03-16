using Roamio.Mobile.Models;

namespace Roamio.Mobile.Views;

public partial class FinalizedDayPlansPage : ContentPage
{
	public FinalizedDayPlansPage()
	{
		InitializeComponent();
	}

    protected override void OnAppearing()
    {
		base.OnAppearing();
		var currentTrip = TripDataStore.CurrentTrip;
		if (currentTrip?.DayPlans != null && currentTrip.DayPlans.Any())
		{
			foreach (var day in currentTrip.DayPlans)
			{
				if (string.IsNullOrEmpty(day.Summary))
				{
					day.Summary = string.Join(", ", day.Schedule.Select(s => s.Name)); // copilot suggestion
                    // day.Summary = $"{day.Schedule.Count} items scheduled"; // chatgpt suggestion
                }
			}
			DayPlansCollection.ItemsSource = currentTrip.DayPlans;
        }
		else 
		{
            DisplayAlert("No Plans", "No day plans found.", "OK");
        }
    }

	private async void OnStartDayClicked(object sender, EventArgs e)
	{
		if (sender is Button btn && btn.CommandParameter is DayPlan selectedDay)
		{
			await Navigation.PushAsync(new DetailedDayPlanPage(selectedDay));
        }
	}
}