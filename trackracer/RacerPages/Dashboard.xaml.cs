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


    public async void OnchatClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new ChatHub());
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

                            if (item.Status != TrackingStatus.Canceled.ToString())
                            { 

                            if (item.Status != TrackingStatus.Approved.ToString())
                            {


                                    // Create ScrollView
                                    var scrollView = new ScrollView
                                    {
                                        Content = new StackLayout
                                        {
                                            Padding = 15,
                                            Spacing = 10,
                                            BackgroundColor = Color.FromArgb("#ff5733") // Main background color
                                        }
                                    };

                                    // Get the inner layout from the ScrollView
                                    var dynamicLayout = (StackLayout)scrollView.Content;

                                    // Create UI elements dynamically
                                    var payLabel = new Label
                                    {
                                        Text = $"Pay: {item.Pay}",
                                        FontSize = 14  ,
                                        FontAttributes = FontAttributes.Bold,
                                        TextColor = Colors.Black // Changed to Black
                                    };
                                    var senderNameLabel = new Label
                                    {
                                        Text = $"Sender: {item.SenderName}",
                                        FontSize = 16,
                                        FontAttributes = FontAttributes.Bold,
                                        TextColor = Colors.Black // Changed to Black
                                    };
                                    var requestTextLabel = new Label
                                    {
                                        Text = $"Request: {item.Text}",
                                        FontSize = 14,
                                        FontAttributes = FontAttributes.Bold,
                                        TextColor = Colors.Black // Changed to Black
                                    };
                                    var requestLocationLabel = new Label
                                    {
                                        Text = $"Location: {item.Location}",
                                        FontSize = 14,
                                        FontAttributes = FontAttributes.Bold,
                                        TextColor = Colors.Black // Changed to Black
                                    };
                            
                                    var statusLabel = new Label
                                    {
                                        Text = $"Status: {item.Status}",
                                        FontSize = 14,
                                        FontAttributes = FontAttributes.Bold,
                                        TextColor = Colors.Black // Changed to Black
                                    };

                                    // Glassmorphism effect frame
                                    var requestFrame = new Frame
                                    {
                                        BackgroundColor = Color.FromArgb("#FFFFFF"), // White background with opacity
                                        Opacity = 0.85, // Slight transparency
                                        Padding = 15,
                                        CornerRadius = 20,
                                        HasShadow = true,
                                        Content = new VerticalStackLayout
                                        {
                                            Spacing = 10,
                                            Children =
                                            {
                                                senderNameLabel,
                                                requestTextLabel,
                                                payLabel,
                                                requestLocationLabel,
                                                statusLabel
                                            }
                                        }
                                    };

                                    // Accept button with theme colors
                                    var acceptButton = new Button
                                    {
                                        Text = "Accept",
                                        HeightRequest = 50,
                                        FontAttributes = FontAttributes.Bold
                                    };
                                    acceptButton.SetAppThemeColor(Button.BackgroundColorProperty, Color.FromArgb("#33ffbd"), Color.FromArgb("#2cbfae")); // Bright green accent
                                    acceptButton.SetAppThemeColor(Button.TextColorProperty, Colors.White, Colors.LightGray);
                                    acceptButton.Clicked += (sender, e) => OnAllowClicked(sender, item.ID, item.Text, item.Location, item.Pay);

                                    // Add elements to the layout inside the ScrollView
                                    dynamicLayout.Children.Add(requestFrame);
                                    dynamicLayout.Children.Add(acceptButton);

                                    // Set the ScrollView as the main page content
                                    this.Content = scrollView;

                                    //     rootLayout.Children.Add(denyButton);
                                }

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



    private async void OnAllowClicked(object sender, int id , string text , string location,int pay)
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
            Location = location,
            Pay = pay
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
                    await DisplayAlert("Success", "Accepted request, You must Complete the request ", "OK");
                }
            }
            else
            {
                await DisplayAlert("Error", $"Failed to send tracking request", "OK");
            }
        }
    }

    //private void OnDenyClicked(object sender, int id, string text, string location)
    //{
    //    var rootLayout = (StackLayout)this.Content;
    //    var button = sender as Button;

    //    if (button != null)
    //    {
    //        // Find the top-most container for this request
    //        var parentLayout = button.Parent as StackLayout;

    //        // Traverse further up if needed
    //        while (parentLayout != null && !rootLayout.Children.Contains(parentLayout))
    //        {
    //            parentLayout = parentLayout.Parent as StackLayout;
    //        }

    //        // Remove the correct parent layout that contains the request
    //        if (parentLayout != null)
    //        {
    //            rootLayout.Children.Remove(parentLayout);
    //        }
    //    }

    //    // Confirmation message
    //    DisplayAlert("Success", "Request removed from your view.", "OK");
    //}
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