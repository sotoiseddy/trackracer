using Newtonsoft.Json;
using System.Net.Http.Json;
using trackracer.Models;
using trackracer.RacerPages;
namespace trackracer.RacerPages;

public partial class Registrationn : ContentPage
{
    private Dictionary<string, int> userTypes = new Dictionary<string, int>
    {
        { "Customer", 1 },
        { "Electrician", 2 },
        { "Carpenter", 3 },
        { "Painter", 4 },
        { "Gardener", 5 },
        { "Locksmith", 6 },
          { "Plumber", 7 }

    };
    private int selectedType = 0; // Default value
    public Registrationn()
	{
		InitializeComponent();
        foreach (var type in userTypes.Keys)
        {
            TypePicker.Items.Add(type);
        }
    }

    private void OnTypeSelected(object sender, EventArgs e)
    {
        if (TypePicker.SelectedIndex != -1)
        {
            string selectedKey = TypePicker.SelectedItem.ToString();
            selectedType = userTypes[selectedKey]; // Get corresponding numeric value
        }
    }
    private async void OnClick(object sender, EventArgs e)
    {
        await Application.Current.MainPage.Navigation.PushModalAsync(new Login());

    }
    private async void OnSignUpClicked(object sender, EventArgs e)
    {
        string username = UsernameEntry.Text;
        string password = PasswordEntry.Text;
        string confirmPassword = ConfirmPasswordEntry.Text;

        if (string.IsNullOrEmpty(username))
        {
            await DisplayAlert("Error", "Please fill in Username", "OK");
            return;
        }
        if (string.IsNullOrEmpty(password))
        {
            await DisplayAlert("Error", "Please fill in Password", "OK");
            return;
        }
        if (string.IsNullOrEmpty(confirmPassword))
        {
            await DisplayAlert("Error", "Please fill in ConfirmPassword", "OK");
            return;
        }

        if (password != confirmPassword)
        {
            await DisplayAlert("Error", "Passwords do not match.", "OK");
            return;
        }
        if (selectedType == 0)
        {
            await DisplayAlert("Error", "Please select a user type.", "OK");
            return;
        }



        RegistrationModel model = new RegistrationModel();
        model.UserID = Guid.NewGuid();
        model.UserName = username;
        model.Password = password;
        model.Type = selectedType;

        using (var client = new HttpClient())// httpClient help to call API
        {

            string url = "http://localhost:5010/api/Account/";
            client.BaseAddress = new Uri(url);
            var resut = client.PostAsJsonAsync<RegistrationModel>("AddUser", model);
            resut.Wait();
            var newResult = resut.Result;
            if (newResult != null && newResult.IsSuccessStatusCode)
            {
                var readtask = newResult.Content.ReadAsStringAsync().Result;
                var finalyResult = JsonConvert.DeserializeObject<bool>(readtask);
                if (finalyResult==true)
                {
                    await DisplayAlert("Success", "Sign up successful!", "OK");
                    signupbtn.IsEnabled = false;
                    await Application.Current.MainPage.Navigation.PushModalAsync(new Login());
                }
                else if (finalyResult == false) 
                {
                await DisplayAlert("Success", "Sign up FAILED, try changing username!", "OK");

                }
            }
            else
            {
                await DisplayAlert("Success", "Sign up FAILED, try changing username!", "OK");

            }


        }
        // Add your sign-up logic here (e.g., save to database, authenticate, etc.)


        // Navigate to the next page or reset the form as needed
    }












}