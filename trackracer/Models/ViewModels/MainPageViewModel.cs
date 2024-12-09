using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using trackracer.Services;


namespace trackracer.Models.ViewModels
{
    public partial class MainPageViewModel  :ObservableObject
    {

        public readonly LocationSyncService _locationSyncService;

        [ObservableProperty]
        public double latitude;
        [ObservableProperty]
        public double longitude;
        [ObservableProperty]
        public bool isListening;
        [ObservableProperty]
        public string? listeningButtonText;


        public MainPageViewModel(LocationSyncService locationSyncService)
        {
            
            _locationSyncService = locationSyncService;
            WeakReferenceMessenger.Default.Register<DeviceLocation>(this, (sender, deviceLocation) =>
            {
                Latitude =  latitude = deviceLocation.Latitude;
                Longitude = longitude = deviceLocation.Longitude;
            });
            listeningButtonText = "Start";
        }
        [RelayCommand]

        private void ChangeListeningMode()
        {
            if (!IsListening)
            {
                _ = _locationSyncService.Start();
                IsListening = true;
                ListeningButtonText = "Stop";
            }
            else
            {
                _locationSyncService.Stop();
                IsListening = false;
                ListeningButtonText = "Start";

           
            }
        }
    }
}
