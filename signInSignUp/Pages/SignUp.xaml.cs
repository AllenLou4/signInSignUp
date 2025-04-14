namespace signInSignUp.Pages
{
    using signInSignUp.Helpers;
    using System;
    using System.Net.Http;
    using System.Text;
    using System.Text.Json;
    using System.Threading.Tasks;
    using Microsoft.Maui.Controls;


    public partial class SignUpPage : ContentPage
    {
        bool isPasswordVisible = false;

        public SignUpPage()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
        }

        private async void OnSignUpButtonClicked(object sender, EventArgs e)
        {
            string email = EmailEntry.Text?.Trim();
            string firstName = FirstNameEntry.Text?.Trim();
            string lastName = LastNameEntry.Text?.Trim();
            string password = PasswordEntry.Text;
            string confirmPassword = PasswordReentry.Text;

            if (string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(firstName) ||
                string.IsNullOrWhiteSpace(lastName) ||
                string.IsNullOrWhiteSpace(password) ||
                string.IsNullOrWhiteSpace(confirmPassword))
            {
                await DisplayAlert("Error", "Please fill in all fields.", "OK");
                return;
            }

            if (password != confirmPassword)
            {
                await DisplayAlert("Error", "Passwords do not match.", "OK");
                return;
            }

            try
            {
                using var client = new HttpClient();
                string url = "https://todo-list.dcism.org/signup_action.php";

                var content = new StringContent(JsonSerializer.Serialize(new
                {
                    first_name = firstName,
                    last_name = lastName,
                    email = email,
                    password = password,
                    confirm_password = confirmPassword
                }), Encoding.UTF8, "application/json");

                var response = await client.PostAsync(url, content);
                var jsonResponse = await response.Content.ReadAsStringAsync();

                var result = JsonSerializer.Deserialize<JsonElement>(jsonResponse);

                int status = result.GetProperty("status").GetInt32();

                if (status == 200)
                {
                    await DisplayAlert("Success", result.GetProperty("message").GetString(), "OK");
                    await Navigation.PopAsync(); // Go back to Sign In page
                }
                else
                {
                    await DisplayAlert("Error", result.GetProperty("message").GetString(), "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"An error occurred:\n{ex.Message}", "OK");
            }
        }


        private async void OnSignInNav(object sender, TappedEventArgs e)
        {
            await Navigation.PopAsync();
        }

        private void TogglePass_Clicked(object sender, EventArgs e)
        {
            var button = sender as ImageButton;
            var entry = button?.CommandParameter as Entry;
            if (entry != null)
            {
                Utility.TogglePasswordVisibility(entry, ref isPasswordVisible);
            }
        }
    }
}