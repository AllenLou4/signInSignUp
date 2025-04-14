using System.Net.Http;
using System.Text;
using System.Text.Json;
using signInSignUp.Helpers;

namespace signInSignUp.Pages
{
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

            string firstName = FirstNameEntry.Text;
            string lastName = LastNameEntry.Text;
            string email = EmailEntry.Text;
            string password = PasswordEntry.Text;
            string confirmPassword = ConfirmPasswordEntry.Text;

            if (string.IsNullOrWhiteSpace(firstName) || string.IsNullOrWhiteSpace(lastName) ||
                string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password) ||
                string.IsNullOrWhiteSpace(confirmPassword))
            {
                await DisplayAlert("Error", "All fields are required.", "OK");
                return;
            }

            if (password != confirmPassword)
            {
                await DisplayAlert("Error", "Passwords do not match.", "OK");
                return;
            }

            var signUpData = new
            {
                first_name = firstName,
                last_name = lastName,
                email = email,
                password = password,
                confirm_password = confirmPassword
            };

            string jsonData = JsonSerializer.Serialize(signUpData);
            var content = new StringContent(jsonData, Encoding.UTF8, "application/json");

            using (var client = new HttpClient())
            {
                try
                {
                    string url = "https://todo-list.dcism.org/signup_action.php";
                    var response = await client.PostAsync(url, content);
                    string responseContent = await response.Content.ReadAsStringAsync();

                    var responseJson = JsonSerializer.Deserialize<SignUpResponse>(responseContent);

                    if (responseJson != null && responseJson.status == 200)
                    {
                        await DisplayAlert("Success", responseJson.message, "OK");
                        await Navigation.PopAsync();
                    }
                    else
                    {
                        await DisplayAlert("Error", responseJson?.message ?? "An error occurred.", "OK");
                    }
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
                }
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

    public class SignUpResponse
    {
        public int status { get; set; }
        public string? message { get; set; }
    }
}