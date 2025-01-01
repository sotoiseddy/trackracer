using System.Net.Http.Json;
using System.Security.AccessControl;

using trackracer.Models;


namespace trackracer.RacerPages;

public partial class Dashboard : ContentPage
{
    Guid? receiverId;

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
                string url = "http://localhost:5010/api/TrackingRequestStatus/";
                client.BaseAddress = new Uri(url);

                var result = await client.GetFromJsonAsync<bool>($"GetTrackingRequestByReceiverID?receiverId={receiverId}");
             
              

                if (result)
                {                  

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