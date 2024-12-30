using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net.Http;
using System.Net.Http.Json;
using trackracer.Models;

namespace trackracer.RacerPages;

public partial class SearchUsersPage : ContentPage
{
    private SearchViewModel ViewModel { get; }// => BindingContext as SearchViewModel;
    Guid? senderID = new Guid();
    Guid? receiverID = new Guid();
    public SearchUsersPage()
    {
        InitializeComponent();
        ViewModel = new SearchViewModel();
        BindingContext = ViewModel;

        senderID = Guid.Parse(Preferences.Get("userID", "default_value"));
    }

    // Triggered when the search text changes in the SearchBar
    private async void OnSearchTextChanged(object sender, TextChangedEventArgs e)
    {
        ViewModel?.FilterItems(e.NewTextValue);

    }

    private void OnItemSelected(object sender, SelectionChangedEventArgs e)
    {
        if (e.CurrentSelection.FirstOrDefault() is RegistrationModel selectedItem)
        {
            // Handle the selected item (e.g., navigate or show details)
            receiverID = selectedItem.UserID;
            DisplayAlert("Selected", selectedItem.UserName + "-" + receiverID, "OK");
        }
    }

    public async void Sendtrackrequest(object sender, EventArgs e)
    {

        try
        {

            // Create the tracking request object
            var trackingRequest = new TrackingRequestStatusModel
            {
                SenderID = senderID,
                ReceiverID = receiverID,
                Status = "Pending" // Approved
            };


            using (var client = new HttpClient())
            {
                string baseUrl = "http://localhost:5010/api/TrackingRequestStatus"; // API endpoint for tracking requests
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
        catch (Exception ex)
        {
            // Display exception details for debugging
            await DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
        }
    }

}







public class SearchViewModel : INotifyPropertyChanged
{
    private List<RegistrationModel> _items;
    private ObservableCollection<RegistrationModel> _filteredItems;
    private string _searchText;

    public List<RegistrationModel> Items
    {
        get => _items;
        set
        {
            _items = value;
            OnPropertyChanged(nameof(Items));
        }
    }

    public ObservableCollection<RegistrationModel> FilteredItems
    {
        get => _filteredItems;
        set
        {
            _filteredItems = value;
            OnPropertyChanged(nameof(FilteredItems));
        }
    }

    public string SearchText
    {
        get => _searchText;
        set
        {
            if (_searchText != value)
            {
                _searchText = value;
                OnPropertyChanged(nameof(SearchText));
                FilterItems(_searchText); // Dynamically filter items
            }
        }
    }

    public SearchViewModel()
    {
        LoadItems();
    }

    private async Task<bool> LoadItems()
    {
        try
        {
            using (var client = new HttpClient())
            {
                string url = "http://localhost:5010/api/Account/";
                client.BaseAddress = new Uri(url);

                var result = await client.GetFromJsonAsync<List<RegistrationModel>>($"GetAllStudents");

                if (result != null && result.Count > 0)
                {
                    Items = result.ToList();
                    FilteredItems = new ObservableCollection<RegistrationModel>(Items);
                    return true;
                }
                else
                {
                    Items = new List<RegistrationModel>();
                    FilteredItems = new ObservableCollection<RegistrationModel>(Items);
                    return true;
                }
            }
        }
        catch (Exception)
        {
            return false;
        }
    }

    public void FilterItems(string searchText)
    {
        if (string.IsNullOrWhiteSpace(searchText))
        {
            FilteredItems = new ObservableCollection<RegistrationModel>(Items); // Show all items if search is empty
        }
        else
        {
            var filtered = _items
                .Where(item => item.UserName.Contains(searchText, StringComparison.OrdinalIgnoreCase))
                .ToList();

            FilteredItems = new ObservableCollection<RegistrationModel>(filtered);
        }
    }

    protected void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public event PropertyChangedEventHandler? PropertyChanged;

}


