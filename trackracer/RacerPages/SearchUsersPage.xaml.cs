using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Json;
using trackracer.Models;

namespace trackracer.RacerPages;

public partial class SearchUsersPage : ContentPage
{
    public SearchUsersPage()
    {
        InitializeComponent();
    }

    // Triggered when the search text changes in the SearchBar
    private async void OnSearchTextChanged(object sender, TextChangedEventArgs e)
    {
        string query = e.NewTextValue;

        if (!string.IsNullOrEmpty(query))
        {
            // Call the backend to search for users
            var results = await SearchUsers(query);
            SearchResultsCollectionView.ItemsSource = results;
        }
        else
        {
            // If no query, clear the results
            SearchResultsCollectionView.ItemsSource = new List<RegistrationModel>();
        }
    }

    // Method to search users from the backend API
    private async Task<List<RegistrationModel>> SearchUsers(string query)
    {
        try
        {
            string url = $"http://localhost:7254/api/users/Search?query={query}";
            using var client = new HttpClient();
            var response = await client.GetStringAsync(url);

            // Deserialize the JSON response into a list of UserModel
            return JsonConvert.DeserializeObject<List<RegistrationModel>>(response);
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
            return new List<RegistrationModel>(); // Return an empty list in case of error
        }
    }
}
