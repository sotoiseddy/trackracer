using System.Collections.ObjectModel;
using System.Globalization;
using Newtonsoft.Json;
using System.Text;
using trackracer.Models;
using Microsoft.AspNetCore.SignalR.Client;
using System.Threading.Tasks;

namespace trackracer.RacerPages;

public partial class ChatPage : ContentPage
{
    public ObservableCollection<ChatModel> Messages { get; set; }
    public string ReceiverName { get; set; }
    public string SenderName { get; set; }
    public string CurrentUserName { get; set; } = Preferences.Get("username", "Guest");
    private HubConnection _hubConnection;

    public ChatPage(string receiverName)
    {
        InitializeComponent();

        Messages = new ObservableCollection<ChatModel>();
        BindingContext = this;

        ReceiverName = receiverName;
        SenderName = CurrentUserName;
        Title = $"Chat with {ReceiverName}";

        _ = InitializeSignalR();
        _ = LoadMessagesAsync();
    }

    private async Task InitializeSignalR()
    {
        _hubConnection = new HubConnectionBuilder()
            .WithUrl("http://localhost:5010/chatsignalhub") // 
            .WithAutomaticReconnect()
            .Build();

        _hubConnection.On<string, string, string>("ReceiveMessage", (senderId, receiverId, messageText) =>
        {
            var myId = Preferences.Get("userID", "default_id");
            var theirId = Preferences.Get("receiverID", "default_id");

            if ((senderId == myId && receiverId == theirId) ||
                (senderId == theirId && receiverId == myId))
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    Messages.Add(new ChatModel
                    {
                        SenderId = senderId,
                        ReceiverId = receiverId,
                        SenderName = senderId == myId ? CurrentUserName : ReceiverName,
                        ReceiverName = senderId == myId ? ReceiverName : CurrentUserName,
                        ChatMessage = messageText,
                        ChatId = Guid.NewGuid().ToString()
                    });
                });
            }
        });

        try
        {
            await _hubConnection.StartAsync();
        }
        catch (Exception ex)
        {
            await DisplayAlert("SignalR Error", ex.Message, "OK");
        }
    }

    private async void OnSendButtonClicked(object sender, EventArgs e)
    {
        var messageText = MessageEntry.Text;
        if (string.IsNullOrWhiteSpace(messageText)) return;

        var message = new ChatModel
        {
            SenderId = Preferences.Get("userID", "default_id"),
            ReceiverId = Preferences.Get("receiverID", "default_id"),
            SenderName = CurrentUserName,
            ReceiverName = ReceiverName,
            ChatMessage = messageText,
            ChatId = Guid.NewGuid().ToString()
        };

        try
        {
            using var client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:5010/");
            var json = JsonConvert.SerializeObject(message);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync("api/Chat/send", content);

            if (response.IsSuccessStatusCode)
            {
               // Messages.Add(message);
                MessageEntry.Text = string.Empty;

                await _hubConnection.InvokeAsync("SendMessage", message.SenderId, message.ReceiverId, message.ChatMessage);
            }
            else
            {
                await DisplayAlert("Error", "Failed to send message to server.", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Send failed: {ex.Message}", "OK");
        }
    }

    private async Task LoadMessagesAsync()
    {
        try
        {
            using var client = new HttpClient();
            client.BaseAddress = new Uri("http://localhost:5010/");

            var senderId = Preferences.Get("userID", "default_id");
            var receiverId = Preferences.Get("receiverID", "default_id");

            var response = await client.GetAsync($"api/Chat/history?user1Id={senderId}&user2Id={receiverId}");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                var messages = JsonConvert.DeserializeObject<List<ChatModel>>(json);

                Messages.Clear();
                foreach (var msg in messages)
                    Messages.Add(msg);
            }
            else
            {
                await DisplayAlert("Error", "Could not load chat history.", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Loading failed: {ex.Message}", "OK");
        }
    }

    protected override async void OnDisappearing()
    {
        if (_hubConnection != null && _hubConnection.State == HubConnectionState.Connected)
        {
            await _hubConnection.StopAsync();
            await _hubConnection.DisposeAsync();
        }

        base.OnDisappearing();
    }

    public class SenderNameColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var senderName = value as string;
            var myName = Preferences.Get("username", "Guest");
            return senderName == myName ? Color.FromHex("#00FF00") : Color.FromHex("#FF0000");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
