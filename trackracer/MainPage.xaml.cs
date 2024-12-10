using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using trackracer.Models;
using trackracer.Models.ViewModels;
using trackracer.Services;

namespace trackracer
{
    public partial class MainPage : ContentPage 
    {
       
        public double latitude;      
        public double longitude;

        public MainPage()
        {
            InitializeComponent();
            // BindingContext = new MainPageViewModel(new LocationSyncService());
            Start();

            
        }

        public async Task Start()
        {
            Geolocation.LocationChanged += Geolocation_LocationChanged;
            var request = new GeolocationListeningRequest(GeolocationAccuracy.Best,
                TimeSpan.FromSeconds(1));
            if (request is not null)
            {
             var   _isListening = await Geolocation.StartListeningForegroundAsync(request);
            }

        }

        private void Geolocation_LocationChanged(object? sender, GeolocationLocationChangedEventArgs e)
        {
            var deviceLocation = new DeviceLocation(e.Location.Latitude, e.Location.Longitude);
           // WeakReferenceMessenger.Default.Send(deviceLocation);

            latitude = e.Location.Latitude;
            longitude = e.Location.Longitude;
            // _isListening = true;

            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await DisplayAlert("Location =", $"{latitude}, {longitude}", "OK");
            });
        }
    }

}
