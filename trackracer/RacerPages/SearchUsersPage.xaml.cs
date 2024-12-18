using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Json;
using trackracer.Models;
using trackracer.RacerPages;
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
            SearchResultsCollectionView.ItemsSource = new List<UserModel>();
        }
    }

    // Method to search users from the backend API
    private async Task<List<UserModel>> SearchUsers(string query)
    {
        try
        {
            string url = $"http://localhost:7254/api/users/Search?query={query}";
            using var client = new HttpClient();
            var response = await client.GetStringAsync(url);

            // Deserialize the JSON response into a list of UserModel
            return JsonConvert.DeserializeObject<List<UserModel>>(response);
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
            return new List<UserModel>(); // Return an empty list in case of error
        }
    }
}
