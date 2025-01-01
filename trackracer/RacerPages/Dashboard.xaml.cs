using System.Net.Http.Json;
using System.Security.AccessControl;

using trackracer.Models;


namespace trackracer.RacerPages;

public partial class Dashboard : ContentPage
{
	public Dashboard()
	{
		InitializeComponent();
	}

	private async void OnClicked(object sender, EventArgs e)
	{
        await Application.Current.MainPage.Navigation.PushModalAsync(new SearchUsersPage());

    }

	public async Task<bool> CallRequests(string TrackingRequest )
	{


        try
        {
            using (var client = new HttpClient())
            {
                string url = "http://localhost:5010/api/TrackingRequestStatus/GetTrackingRequestByReceiverID";
                client.BaseAddress = new Uri(url);

                // Check if the user exists and is authenticated
                var result = await client.GetFromJsonAsync<bool>(TrackingRequest);
              

                if (result)
                {
                    // Retrieve user details
                     await client.GetFromJsonAsync<TrackingRequestStatusModel>(TrackingRequest);
                 

                    // Store user details locally
                    Preferences.Set("Request", TrackingRequest);
                    


                    return true;
                }
                else
                {
                    await DisplayAlert("Error", "Login FAILED", "OK");
                    return false;
                }
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
            return false;
        }



    }
}