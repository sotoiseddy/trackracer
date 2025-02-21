using System.Net.Http.Json;
using Newtonsoft.Json;
using trackracer.Models;
using static trackracer.Models.StatusDetails;

namespace trackracer.RacerPages;

public partial class CustomerDashboard : ContentPage
{
    private Guid UserID;

    public CustomerDashboard()
    {
        InitializeComponent();
        UserID = Guid.Parse(Preferences.Get("userID", Guid.Empty.ToString()));
        CallRequests();
    }

    public async Task CallRequests()
    {
        try
        {
            using (var client = new HttpClient())
            {
                string url = "http://localhost:5010/api/TrackingRequestStatus/";
                client.BaseAddress = new Uri(url);

                var result = await client.GetFromJsonAsync<List<TrackingRequestStatusModel>>("GetAllRequestStatuses");

                if (result != null)
                {
                    RequestListLayout.Children.Clear(); // Clear previous data

                    foreach (var item in result)
                    {
                        if (item.Status != TrackingStatus.Canceled.ToString() && item.SenderID == UserID)
                        {
                            var container = new Frame
                            {
                                CornerRadius = 10,
                                Padding = 10,
                                BackgroundColor = Colors.White,
                                BorderColor = Colors.Black
                            };

                            var contentStack = new VerticalStackLayout
                            {
                                Spacing = 5
                            };

                            contentStack.Children.Add(new Label { Text = $"Pay: {item.Pay}", FontAttributes = FontAttributes.Bold, TextColor = Colors.Black });
                            contentStack.Children.Add(new Label { Text = $"Sender: {item.SenderName}", FontAttributes = FontAttributes.Bold, TextColor = Colors.Black });
                            contentStack.Children.Add(new Label { Text = $"Request: {item.Text}", FontAttributes = FontAttributes.Bold, TextColor = Colors.Black });
                            contentStack.Children.Add(new Label { Text = $"Location: {item.Location}", FontAttributes = FontAttributes.Bold, TextColor = Colors.Black });
                            contentStack.Children.Add(new Label { Text = $"Status: {item.Status}", FontAttributes = FontAttributes.Bold, TextColor = Colors.Black });

                            var cancelButton = new Button
                            {
                                Text = "Cancel",
                                BackgroundColor = Colors.Red,
                                TextColor = Colors.White,
                                IsEnabled = item.Status != TrackingStatus.Approved.ToString()
                            };

                            if (cancelButton.IsEnabled)
                            {
                                cancelButton.Clicked += (sender, e) => OnCanceledClicked(sender, item.ID, item.Text, item.Location);
                            }

                            contentStack.Children.Add(cancelButton);
                            container.Content = contentStack;
                            RequestListLayout.Children.Add(container);
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

    private async void OnCanceledClicked(object sender, int id, string text, string location)
    {
        var trackingRequest = new TrackingRequestStatusModel
        {
            ID = id,
            SenderID = new Guid(),
            SenderName = "",
            Status = TrackingStatus.Canceled.ToString(),
            Text = text,
            Location = location
        };

        using (var client = new HttpClient())
        {
            string baseUrl = "http://localhost:5010/api/TrackingRequestStatus/";
            client.BaseAddress = new Uri(baseUrl);
            var result = await client.PostAsJsonAsync("SaveRequest", trackingRequest);

            if (result.IsSuccessStatusCode)
            {
                var readTask = await result.Content.ReadAsStringAsync();
                var finalResult = JsonConvert.DeserializeObject<bool>(readTask);
                if (finalResult)
                {
                    await DisplayAlert("Success", "Request canceled successfully!", "OK");
                    await CallRequests(); // Refresh list
                }
            }
            else
            {
                await DisplayAlert("Error", "Failed to cancel request.", "OK");
            }
        }
    }

    private async void OnLoginnClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new SearchUsersPage());
    }
}
