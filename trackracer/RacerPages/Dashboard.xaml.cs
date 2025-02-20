using System.Net.Http.Json;
using System.Security.AccessControl;

using Newtonsoft.Json;
using trackracer.Models;
using static System.Net.Mime.MediaTypeNames;
using static trackracer.Models.StatusDetails;


namespace trackracer.RacerPages;

public partial class Dashboard : ContentPage
{
  
    

    public Dashboard()
    {
        InitializeComponent();
       
   
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

                var result = await client.GetFromJsonAsync<List<TrackingRequestStatusModel>>($"GetAllRequestStatuses");

                if (result != null )
                {
                    // Loop through the requests and render only those with a status that is not "Rejected"
                    foreach (var item in result)
                    {
                        int userType = Preferences.Get("UserType", 0);
                        if (item.RequestType == userType)
                        {


                            if (item.Status != TrackingStatus.Rejected.ToString())
                            {



                                var senderNameLabel = new Label
                                {
                                    Text = $"Sender: {item.SenderName}",
                                    FontSize = 16,
                                    FontAttributes = FontAttributes.Bold
                                };
                                var requestTextLabel = new Label
                                {
                                    Text = $"Request: {item.Text}",
                                    FontSize = 14,
                                    FontAttributes = FontAttributes.Italic
                                };
                                var requestLocationLabel = new Label
                                {
                                    Text = $"Request: {item.Location}",
                                    FontSize = 14,
                                    FontAttributes = FontAttributes.Italic
                                };
                                var senderIdLabel = new Label
                                {
                                    Text = $"Sender ID: {item.SenderID}",
                                    FontSize = 14
                                };
                                var statusLabel = new Label
                                {
                                    Text = $"Status: {item.Status}",
                                    FontSize = 14,
                                    FontAttributes = FontAttributes.Italic
                                };
                                var acceptButton = new Button
                                {
                                    Text = "Accept"
                                };
                                acceptButton.SetAppThemeColor(Button.BackgroundColorProperty, Colors.Green, Colors.DarkGreen); 
                                acceptButton.SetAppThemeColor(Button.TextColorProperty, Colors.White, Colors.LightGray);
                                acceptButton.Clicked += (sender, e) => OnAllowClicked(sender, item.ID, item.Text ,item.Location);
                                var denyButton = new Button
                                {
                                    Text = "Deny"
                                };
                                denyButton.SetAppThemeColor(Button.BackgroundColorProperty, Colors.Red, Colors.DarkRed);
                                denyButton.SetAppThemeColor(Button.TextColorProperty, Colors.White, Colors.LightGray);
                                denyButton.Clicked += (sender, e) => OnDenyClicked(sender, item.ID, item.Text, item.Location);
                                var rootLayout = (StackLayout)this.Content;
                                rootLayout.Children.Add(senderNameLabel);
                                rootLayout.Children.Add(requestTextLabel);
                                rootLayout.Children.Add(requestLocationLabel);
                                rootLayout.Children.Add(senderIdLabel);
                                rootLayout.Children.Add(statusLabel);
                                rootLayout.Children.Add(acceptButton);
                                rootLayout.Children.Add(denyButton);
                            }
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



    private async void OnAllowClicked(object sender, int id , string text , string location)
    {
        var trackingRequest = new TrackingRequestStatusModel
        {
            ID = id,
            SenderID = new Guid(),
            //ReceiverID = new Guid(),
            SenderName = "",
           // ReceiverName = "",
            Status = TrackingStatus.Approved.ToString(),
            Text = text,
            Location = location
        };

        using (var client = new HttpClient())
        {
            string baseUrl = "http://localhost:5010/api/TrackingRequestStatus/";
            client.BaseAddress = new Uri(baseUrl);
            var result = await client.PostAsJsonAsync<TrackingRequestStatusModel>("SaveRequest", trackingRequest);

            if (result.IsSuccessStatusCode)
            {
                var readTask = await result.Content.ReadAsStringAsync();
                var finalResult = JsonConvert.DeserializeObject<bool>(readTask);
                if (finalResult)
                {
                    await DisplayAlert("Success", "Accepted request successfully!", "OK");
                }
            }
            else
            {
                await DisplayAlert("Error", $"Failed to send tracking request", "OK");
            }
        }
    }

    private async void OnDenyClicked(object sender, int id, string text, string location)
    {
        var trackingRequest = new TrackingRequestStatusModel
        {
            ID = id,
            SenderID = new Guid(),
            SenderName = "",
            Status = TrackingStatus.Rejected.ToString(),
            //RequestType =
             Text = text,
             Location = location
        };

        using (var client = new HttpClient())
        {
            string baseUrl = "http://localhost:5010/api/TrackingRequestStatus/";
            client.BaseAddress = new Uri(baseUrl);
            var result = await client.PostAsJsonAsync<TrackingRequestStatusModel>("SaveRequest", trackingRequest);

            if (result.IsSuccessStatusCode)
            {
                var readTask = await result.Content.ReadAsStringAsync();
                var finalResult = JsonConvert.DeserializeObject<bool>(readTask);
                if (finalResult)
                {
                    await DisplayAlert("Success", "Denied request successfully!", "OK");
                }
            }
            else
            {
                await DisplayAlert("Error", $"Failed to send tracking request", "OK");
            }
        }
    }
    //private void RenderRequests(List<TrackingRequestStatusModel> requests)
    //{
    //    var rootLayout = (StackLayout)this.Content;
    //    rootLayout.Children.Clear(); // Clear previous UI elements

    //    foreach (var item in requests)
    //    {
    //        // Sender name label
    //        var senderNameLabel = new Label
    //        {
    //            Text = $"Sender: {item.SenderName}",
    //            FontSize = 16,
    //            FontAttributes = FontAttributes.Bold
    //        };

    //        // Sender ID label
    //        var senderIdLabel = new Label
    //        {
    //            Text = $"Sender ID: {item.SenderID}",
    //            FontSize = 14
    //        };

    //        // Status label
    //        var statusLabel = new Label
    //        {
    //            Text = $"Status: {item.Status}",
    //            FontSize = 14,
    //            FontAttributes = FontAttributes.Italic
    //        };

    //        // Accept button
    //        var acceptButton = new Button
    //        {
    //            Text = "Accept"
    //        };
    //        acceptButton.Clicked += (sender, e) => OnAllowClicked(sender, item.ID , item.Text);

    //        // Deny button
    //        var denyButton = new Button
    //        {
    //            Text = "Deny"
    //        };
    //        denyButton.Clicked += (sender, e) => OnDenyClicked(sender, item.ID  , item.Text);

    //        // Add elements to the layout
    //        rootLayout.Children.Add(senderNameLabel);
    //        rootLayout.Children.Add(senderIdLabel);
    //        rootLayout.Children.Add(statusLabel);
    //        rootLayout.Children.Add(acceptButton);
    //        rootLayout.Children.Add(denyButton);
    //    }
    //}

}