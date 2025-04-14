using signInSignUp.Pages;
using signInSignUp.Helpers;
using System.Net.Http;
using System.Text.Json;


namespace signInSignUp
{
    public partial class MainPage : ContentPage
    {
        bool isPasswordVisible = false;

        public MainPage()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
        }
        protected override bool OnBackButtonPressed(){
            return true;
        }
        private void TogglePass_Clicked(object sender, EventArgs e)
        {
            Utility.TogglePasswordVisibility(PasswordInput, ref isPasswordVisible);
        }
        private void OnSignUpLabelTapped(object sender, EventArgs e)
        {   
            Navigation.PushAsync(new SignUpPage());
        }
        private async void OnSignInButtonClicked(object sender, EventArgs e)
        {
            string email = EmailEntry.Text?.Trim();
            string password = PasswordInput.Text;

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                await DisplayAlert("Error", "Please enter both email and password.", "OK");
                return;
            }

            try
            {
                using var client = new HttpClient();
                string url = $"https://todo-list.dcism.org/signin_action.php?email={Uri.EscapeDataString(email)}&password={Uri.EscapeDataString(password)}";

                var response = await client.GetAsync(url);
                var json = await response.Content.ReadAsStringAsync();

                var result = JsonSerializer.Deserialize<JsonElement>(json);

                int status = result.GetProperty("status").GetInt32();

                if (status == 200)
                {
                    // You can extract user info if needed:
                    var data = result.GetProperty("data");
                    string firstName = data.GetProperty("fname").GetString();

                    await DisplayAlert("Success", $"Welcome back, {firstName}!", "Continue");
                    await Navigation.PushAsync(new ToDoListPage());
                }
                else
                {
                    string message = result.GetProperty("message").GetString();
                    await DisplayAlert("Sign In Failed", message, "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"An error occurred:\n{ex.Message}", "OK");
            }
        }

    }
}