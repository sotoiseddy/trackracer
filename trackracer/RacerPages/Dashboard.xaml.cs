using System.Net.Http.Json;
using System.Security.AccessControl;
using Newtonsoft.Json;
using trackracer.Models;
using static trackracer.Models.StatusDetails;


namespace trackracer.RacerPages;

public partial class Dashboard : ContentPage
{
    Guid? receiverId = new Guid();
    private Guid? senderID;
    private Guid? receiverID;
    private string userName;
    private string receiverName;

    public Dashboard()
    {
        InitializeComponent();
        receiverId = Guid.Parse(Preferences.Get("userID", "default_value"));
        CallRequests();
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

                var result = client.GetFromJsonAsync<TrackingRequestStatusModel>($"GetTrackingRequestByReceiverID?receiverId={receiverId}");

                result.Wait();
                var newResult = result.Result;

                if (newResult != null)
                {
                    if (newResult.ID == 0)
                    {
                        

                    }
                    else
                    {
                        var rootLayout = (StackLayout)this.Content;
                        var senderNameLabel = new Label
                        {
                            Text = $"Sender: {newResult.SenderName}",
                            FontSize = 16,
                            FontAttributes = FontAttributes.Bold
                        };

                        // Create a label for Sender ID
                        var senderIdLabel = new Label
                        {
                            Text = $"Sender ID: {newResult.SenderID}",
                            FontSize = 14
                        };

                        // Create a label for Status
                        var statusLabel = new Label
                        {
                            Text = $"Status: {newResult.Status}",
                            FontSize = 14,
                            FontAttributes = FontAttributes.Italic
                        };

                        // Add these labels to the layout dynamically
                        rootLayout.Children.Add(senderNameLabel);
                        rootLayout.Children.Add(senderIdLabel);
                        rootLayout.Children.Add(statusLabel);





                    }

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

    private async void OnAllowClicked(object sender, EventArgs e)
    {
       
        var trackingRequest = new TrackingRequestStatusModel
        {
            // ID = 01,
            SenderID = senderID,
            ReceiverID = receiverID,
            SenderName = userName,
            ReceiverName = receiverName,
            Status = TrackingStatus.Approved.ToString()
        };


        using (var client = new HttpClient())
        {
            string baseUrl = "http://localhost:5010/api/TrackingRequestStatus/"; // API endpoint for tracking requests
            client.BaseAddress = new Uri(baseUrl);
            var resut = client.PostAsJsonAsync<TrackingRequestStatusModel>("SaveRequest", trackingRequest);

            resut.Wait();
            var newResult = resut.Result;
            if (newResult != null && newResult.IsSuccessStatusCode)
            {
                var readtask = newResult.Content.ReadAsStringAsync().Result;
                var finalyResult = JsonConvert.DeserializeObject<bool>(readtask);
                if (finalyResult)
                {
                    await DisplayAlert("Success", "send tracking request successful!", "OK");
                }

            }
            else
            {

                await DisplayAlert("Error", $"Failed to send tracking request", "OK");
            }
        }

    }
    private async void OnDenyClicked(object sender, EventArgs e)
    {
        return;
        var trackingRequest = new TrackingRequestStatusModel
        {
            // ID = 01,
            SenderID = senderID,
            ReceiverID = receiverID,
            SenderName = userName,
            ReceiverName = receiverName,
            Status = TrackingStatus.Rejected.ToString()
        };


        using (var client = new HttpClient())
        {
            string baseUrl = "http://localhost:5010/api/TrackingRequestStatus/"; // API endpoint for tracking requests
            client.BaseAddress = new Uri(baseUrl);
            var resut = client.PostAsJsonAsync<TrackingRequestStatusModel>("SaveRequest", trackingRequest);

            resut.Wait();
            var newResult = resut.Result;
            if (newResult != null && newResult.IsSuccessStatusCode)
            {
                var readtask = newResult.Content.ReadAsStringAsync().Result;
                var finalyResult = JsonConvert.DeserializeObject<bool>(readtask);
                if (finalyResult)
                {
                    await DisplayAlert("Success", "send tracking request successful!", "OK");
                }

            }
            else
            {

                await DisplayAlert("Error", $"Failed to send tracking request", "OK");
            }
        }
    }
}