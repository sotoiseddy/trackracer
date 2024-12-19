using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net.Http;
using System.Net.Http.Json;
using trackracer.Models;

namespace trackracer.RacerPages;

public partial class SearchUsersPage : ContentPage
{
    private SearchViewModel ViewModel => BindingContext as SearchViewModel;

    public SearchUsersPage()
    {
        InitializeComponent();      
    }

    // Triggered when the search text changes in the SearchBar
    private async void OnSearchTextChanged(object sender, TextChangedEventArgs e)
    {
        ViewModel?.FilterItems(e.NewTextValue);

        //string query = e.NewTextValue;

        //if (!string.IsNullOrEmpty(query))
        //{
        //    // Call the backend to search for users
        //    var results = await SearchUsers(query);
        //    SearchResultsCollectionView.ItemsSource = results;
        //}
        //else
        //{
        //    // If no query, clear the results
        //    SearchResultsCollectionView.ItemsSource = new List<RegistrationModel>();
        //}


    }

    // Method to search users from the backend API
    private async Task<List<RegistrationModel>> SearchUsers(string query)
    {
        try
        {
            //string url = $"http://localhost:7254/api/users/Search?query={query}";
            //using var client = new HttpClient();
            //var response = await client.GetStringAsync(url);

            //// Deserialize the JSON response into a list of UserModel
            //return JsonConvert.DeserializeObject<List<RegistrationModel>>(response);
            return new List<RegistrationModel>();
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
            return new List<RegistrationModel>(); // Return an empty list in case of error
        }
    }
}

public class SearchViewModel : INotifyPropertyChanged
{
    private List<RegistrationModel> _items;
    private List<RegistrationModel> _filteredItems;

    public List<RegistrationModel> Items
    {
        get => _items;
        set
        {
            _items = value;
            OnPropertyChanged(nameof(Items));
        }
    }

    public List<RegistrationModel> FilteredItems
    {
        get => _filteredItems;
        set
        {
            _items = value;
            OnPropertyChanged(nameof(FilteredItems));
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
                string url = "https://localhost:7254/api/Account/";
                client.BaseAddress = new Uri(url);

                // Check if the user exists and is authenticated
                var result = await client.GetFromJsonAsync<List<RegistrationModel>>($"GetAllStudent");

                if (result != null && result.Count > 0)
                {
                    Items = result.ToList();
                    FilteredItems = Items;
                    return true;
                }
                else
                {
                    Items = new List<RegistrationModel>();
                    FilteredItems = Items;
                    return true;
                }
            }
        }
        catch (Exception ex)
        {
           // await DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
            return false;
        }
    }
    
    public void FilterItems(string searchText)
    {
        if(string.IsNullOrWhiteSpace(searchText))
        {
            FilteredItems = Items.ToList();
        }
        else
        {
            FilteredItems = Items.Where (c=>c.UserName.Contains(searchText,StringComparison.OrdinalIgnoreCase)).ToList(); 
        }
    }

    protected void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this,new PropertyChangedEventArgs(propertyName));
    }

    public event PropertyChangedEventHandler? PropertyChanged;


}
