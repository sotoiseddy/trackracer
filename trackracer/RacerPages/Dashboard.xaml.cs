using System.Net.Http.Json;
using System.Security.AccessControl;

using trackracer.Models;


namespace trackracer.RacerPages;

public partial class Dashboard : ContentPage
{
    Guid? receiverId = new Guid();

    public Dashboard()
	{
		InitializeComponent();
        receiverId = Guid.Parse(Preferences.Get("userID", "default_value"));
    }

	private async void OnClicked(object sender, EventArgs e)
	{
        await Application.Current.MainPage.Navigation.PushModalAsync(new SearchUsersPage());

    }

	public async Task CallRequests()
	{

        try
        {
            using (var client = new HttpClient())
            {
                string url = "http://localhost:5010/api/TrackingRequestStatus/";
                client.BaseAddress = new Uri(url);

                var result = await client.GetFromJsonAsync<TrackingRequestStatusModel>($"GetTrackingRequestByReceiverID?receiverId={receiverId}");
             
              

                if (result != null)
                {                  

                   
                }
                else
                {
                    await DisplayAlert("Error", "Login FAILED", "OK");
                   
                }
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
           
        }



    }
}