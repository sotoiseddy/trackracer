using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net.Http;
using System.Net.Http.Json;
using trackracer.Models;

namespace trackracer.RacerPages;

public partial class SearchUsersPage : ContentPage
{
    private SearchViewModel ViewModel { get; }// => BindingContext as SearchViewModel;

    public SearchUsersPage()
    {
        InitializeComponent();
        ViewModel = new SearchViewModel();
        BindingContext = ViewModel;
    }

    // Triggered when the search text changes in the SearchBar
    private async void OnSearchTextChanged(object sender, TextChangedEventArgs e)
    {
        ViewModel?.FilterItems(e.NewTextValue);

    }

}

//public class SearchViewModel : INotifyPropertyChanged
//{
//    private List<RegistrationModel> _items;
//    private List<RegistrationModel> _filteredItems;

//    public List<RegistrationModel> Items
//    {
//        get => _items;
//        set
//        {
//            _items = value;
//            OnPropertyChanged(nameof(Items));
//        }
//    }

//    public List<RegistrationModel> FilteredItems
//    {
//        get => _filteredItems;
//        set
//        {
//            _filteredItems = value;
//            OnPropertyChanged(nameof(FilteredItems));
//        }
//    }

//    public SearchViewModel()
//    {
//        LoadItems();
//    }

//    private async Task<bool> LoadItems()
//    {
//        try
//        {
//            using (var client = new HttpClient())
//            {
//                string url = "https://localhost:7254/api/Account/";
//                client.BaseAddress = new Uri(url);

//                // Check if the user exists and is authenticated
//                var result = await client.GetFromJsonAsync<List<RegistrationModel>>($"GetAllStudents");

//                if (result != null && result.Count > 0)
//                {
//                    Items = result.ToList();
//                    FilteredItems = Items;
//                    return true;
//                }
//                else
//                {
//                    Items = new List<RegistrationModel>();
//                    FilteredItems = Items;
//                    return true;
//                }
//            }
//        }
//        catch (Exception ex)
//        {
//           // await DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
//            return false;
//        }
//    }

//    public void FilterItems(string searchText)
//    {
//        if(string.IsNullOrWhiteSpace(searchText))
//        {
//            //FilteredItems = Items.ToList();
//            FilteredItems = new List<RegistrationModel>(Items);

//        }
//        else
//        {
//            FilteredItems = Items.Where (c=>c.UserName.ToLower().Contains(searchText.ToLower())).ToList();         
//        }
//    }

//    protected void OnPropertyChanged(string propertyName)
//    {
//        PropertyChanged?.Invoke(this,new PropertyChangedEventArgs(propertyName));
//    }

//    public event PropertyChangedEventHandler? PropertyChanged;


//}

public class SearchViewModel : INotifyPropertyChanged
{
    private List<RegistrationModel> _items;
    private List<RegistrationModel> _filteredItems;
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

    public List<RegistrationModel> FilteredItems
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
                string url = "https://localhost:7254/api/Account/";
                client.BaseAddress = new Uri(url);

                var result = await client.GetFromJsonAsync<List<RegistrationModel>>($"GetAllStudents");

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
        catch (Exception)
        {
            return false;
        }
    }

    public void FilterItems(string searchText)
    {
        if (string.IsNullOrWhiteSpace(searchText))
        {
            FilteredItems = new List<RegistrationModel>(Items); // Show all items if search is empty
        }
        else
        {
            FilteredItems = Items
                .Where(c => c.UserName.ToLower().Contains(searchText.ToLower()))
                .ToList();
        }
    }

    protected void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public event PropertyChangedEventHandler? PropertyChanged;
}


