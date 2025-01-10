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

                var result = await client.GetFromJsonAsync<List<TrackingRequestStatusModel>>($"GetTrackingRequestByReceiverID?receiverId={receiverId}");

                if (result != null)
                {
                    // Loop through the requests and render only those with a status that is not "Rejected"
                    foreach (var item in result)
                    {
                        if (item.Status != TrackingStatus.Rejected.ToString())
                        {
                            // Sender name label
                            var senderNameLabel = new Label
                            {
                                Text = $"Sender: {item.SenderName}",
                                FontSize = 16,
                                FontAttributes = FontAttributes.Bold
                            };

                            // Sender ID label
                            var senderIdLabel = new Label
                            {
                                Text = $"Sender ID: {item.SenderID}",
                                FontSize = 14
                            };

                            // Status label
                            var statusLabel = new Label
                            {
                                Text = $"Status: {item.Status}",
                                FontSize = 14,
                                FontAttributes = FontAttributes.Italic
                            };

                            // Accept button
                            var acceptButton = new Button
                            {
                                Text = "Accept"
                            };
                            acceptButton.SetAppThemeColor(Button.BackgroundColorProperty, Colors.Green, Colors.DarkGreen); // Theme-aware color
                            acceptButton.SetAppThemeColor(Button.TextColorProperty, Colors.White, Colors.LightGray);
                            acceptButton.Clicked += (sender, e) => OnAllowClicked(sender, item.ID);

                            // Deny button
                            var denyButton = new Button
                            {
                                Text = "Deny"
                            };
                            denyButton.SetAppThemeColor(Button.BackgroundColorProperty, Colors.Red, Colors.DarkRed);
                            denyButton.SetAppThemeColor(Button.TextColorProperty, Colors.White, Colors.LightGray);
                            denyButton.Clicked += (sender, e) => OnDenyClicked(sender, item.ID);

                            // Add elements to the layout
                            var rootLayout = (StackLayout)this.Content;
                            rootLayout.Children.Add(senderNameLabel);
                            rootLayout.Children.Add(senderIdLabel);
                            rootLayout.Children.Add(statusLabel);
                            rootLayout.Children.Add(acceptButton);
                            rootLayout.Children.Add(denyButton);
                        }
                    }
                }
                else
                {
                    await DisplayAlert("Error", "No requests found.", "OK");
                }
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
        }
    }





    private async void OnAllowClicked(object sender,int id)
    {

        var trackingRequest = new TrackingRequestStatusModel
        {
            ID = id,
            SenderID = new Guid(),
            ReceiverID = new Guid(),
            SenderName = "",
            ReceiverName = "",
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
                    await DisplayAlert("Success", "Accepted request successful!", "OK");
                }

            }
            else
            {

                await DisplayAlert("Error", $"Failed to send tracking request", "OK");
            }
        }

    }
    private async void OnDenyClicked(object sender,int id)
    {

        var trackingRequest = new TrackingRequestStatusModel
        {
            ID = id,
            SenderID = new Guid(),
            ReceiverID = new Guid(),
            SenderName = "",
            ReceiverName = "",
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
                    await DisplayAlert("Success", "Denied request successful!", "OK");
                }

            }
            else
            {

                await DisplayAlert("Error", $"Failed to send tracking request", "OK");
            }
        }
    }

    private void RenderRequests(List<TrackingRequestStatusModel> requests)
    {
        var rootLayout = (StackLayout)this.Content;
        rootLayout.Children.Clear(); // Clear previous UI elements

        foreach (var item in requests)
        {
            // Sender name label
            var senderNameLabel = new Label
            {
                Text = $"Sender: {item.SenderName}",
                FontSize = 16,
                FontAttributes = FontAttributes.Bold
            };

            // Sender ID label
            var senderIdLabel = new Label
            {
                Text = $"Sender ID: {item.SenderID}",
                FontSize = 14
            };

            // Status label
            var statusLabel = new Label
            {
                Text = $"Status: {item.Status}",
                FontSize = 14,
                FontAttributes = FontAttributes.Italic
            };

            // Accept button
            var acceptButton = new Button
            {
                Text = "Accept"
            };
            acceptButton.Clicked += (sender, e) => OnAllowClicked(sender, item.ID);

            // Deny button
            var denyButton = new Button
            {
                Text = "Deny"
            };
            denyButton.Clicked += (sender, e) => OnDenyClicked(sender, item.ID);

            // Add elements to the layout
            rootLayout.Children.Add(senderNameLabel);
            rootLayout.Children.Add(senderIdLabel);
            rootLayout.Children.Add(statusLabel);
            rootLayout.Children.Add(acceptButton);
            rootLayout.Children.Add(denyButton);
        }
    }

}