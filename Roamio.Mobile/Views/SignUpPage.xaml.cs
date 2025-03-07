using Microsoft.Maui.Controls;
using Roamio.Mobile.Models;
using Roamio.Mobile.Services;
using System;
using System.Threading.Tasks;

namespace Roamio.Mobile.Views;

public partial class SignUpPage : ContentPage
{
	private readonly IApiService _apiService;

    public SignUpPage() : this(MauiProgram.Services.GetRequiredService<IApiService>())
    {        
    }

    public SignUpPage(IApiService apiService)
	{
		InitializeComponent();
		_apiService = apiService;
	}

	private async void OnSignUpClicked(object sender, EventArgs e)
	{
		var username = UsernameEntry.Text;
		var password = PasswordEntry.Text;

		if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
		{
			await DisplayAlert("Error", "Please enter both username and password", "OK");
			return;
		}

		//add encryption!!!!!!!!!
		var newUser = new User
		{
			Username = username,
			HashedPassword = password, //add encryption!!!!!!!!!
			Preferences = null,
			TripHistory = null,
			DailyPlans = null
		};

		try
		{
			var createdUser = await _apiService.CreateUserAsync(newUser);
			if (createdUser != null)
			{
                await DisplayAlert("Success", "Account created successfully!", "OK");
            }
			else
			{
                await DisplayAlert("Error", "Account creation failed. Please try again.", "OK");
            }
		}
		catch (Exception ex)
		{
			await DisplayAlert("Error", ex.Message, "OK");
		}
	}
}