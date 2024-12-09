using trackracer.Models.ViewModels;
using trackracer.Services;

namespace trackracer
{
    public partial class MainPage : ContentPage 
    {
      

        public MainPage()
        {
            InitializeComponent();
            BindingContext = new MainPageViewModel(new LocationSyncService());

        }

      
    }

}
