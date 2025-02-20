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

            }//customer
            else if (acctype == 2)
            {

                await Application.Current.MainPage.Navigation.PushModalAsync(new Dashboard());
            }//Electricion
            else if (acctype == 3)
            {

                await Application.Current.MainPage.Navigation.PushModalAsync(new Dashboard());
            }//Carpanter
            else if (acctype == 4)
            {

                await Application.Current.MainPage.Navigation.PushModalAsync(new Dashboard());
            }//Painter
            else if (acctype == 5)
            {

                await Application.Current.MainPage.Navigation.PushModalAsync(new Dashboard());
            }//Gardener
            else if (acctype == 6)
            {

                await Application.Current.MainPage.Navigation.PushModalAsync(new Dashboard());
            }//Locksmith
            else if (acctype == 7)
            {

                await Application.Current.MainPage.Navigation.PushModalAsync(new Dashboard());
            }//Plumber
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
                string url = "http://localhost:5010/api/Account/";
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
                    Preferences.Set("UserType", acctype);

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