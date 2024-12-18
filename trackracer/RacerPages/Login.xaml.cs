using Newtonsoft.Json;

using System.Net.Http.Json;
using trackracer.Models;


namespace trackracer.RacerPages;

public partial class Login : ContentPage
{
	public Login()
	{
		InitializeComponent();
	}

    private async void OnClick(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new MainPage());
    }
    private async void OnLogin1Clicked()
    {
        await Navigation.PushAsync(new MainPage());
    }
    //a
    private async void OnLoginClicked(object sender, EventArgs e)
    {
        string username = UsernameEntry.Text;
        string password = PasswordEntry.Text;

        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            await DisplayAlert("Error", "Please fill in both fields.", "OK");
            return;
        }
        if (username == "admin" && password == "Passwod")
        {
            // await DisplayAlert("Error", "Please fill in both fields.", "OK");
            //await Navigation.PushAsync(new AdminHomePage());
            return;
        }

        bool isAuthenticated = await AuthenticateUser(username, password);

        if (isAuthenticated)
        {
            await DisplayAlert("Success", "Login successful!", "OK");
            if (acctype == 1)
            {
                await Application.Current.MainPage.Navigation.PushModalAsync(new SearchUsersPage());

            }
            else if (acctype == 2)
            {

                await Application.Current.MainPage.Navigation.PushModalAsync(new SearchUsersPage());
            }
            else
            {
                await DisplayAlert("Error", "Invalid Account Type!!!.", "OK");
            }
        }
        else
        {
            await DisplayAlert("Error", "Invalid username or password.", "OK");
        }
    }
    private async void OnSignupClicked(object sender, EventArgs e)
    {
        await Application.Current.MainPage.Navigation.PushModalAsync(new Registrationn());
    }


    RegistrationModel registrationModel = new RegistrationModel();
    int acctype;
    private async Task<bool> AuthenticateUser(string username, string password)
    {
        try
        {
            using (var client = new HttpClient())
            {
                string url = "https://localhost:7254/api/Account/";
                client.BaseAddress = new Uri(url);

                // Check if the user exists and is authenticated
                var result = await client.GetFromJsonAsync<bool>($"Login?username={username}&password={password}");

                if (result)
                {
                    // Retrieve user details
                    registrationModel = await client.GetFromJsonAsync<RegistrationModel>($"GetUser?username={username}");
                    acctype = registrationModel.Type;

                    // Store user details locally
                    Preferences.Set("username", username);
                    Preferences.Set("userID", registrationModel.UserID.ToString());
                    return true;
                }
                else
                {
                    await DisplayAlert("Error", "Login FAILED", "OK");
                    return false;
                }
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
            return false;
        }
    }
}