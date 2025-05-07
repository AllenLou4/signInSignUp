using System.Net.Http;
using System.Text.Json;
using signInSignUp.Pages;
using signInSignUp.Helpers;

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

        protected override bool OnBackButtonPressed()
        {
            return true;
        }

        private void Entry_Focused(object sender, FocusEventArgs e)
        {
            if (sender == EmailEntry)
            {
                EmailBorder.Stroke = Colors.DarkOliveGreen;
                EmailBorder.StrokeThickness = 2;
            }
            else if (sender == PasswordInput)
            {
                PasswordBorder.Stroke = Colors.DarkOliveGreen;
                PasswordBorder.StrokeThickness = 2;
            }
        }

        private void Entry_Unfocused(object sender, FocusEventArgs e)
        {
            var defaultColor = Color.FromArgb("#8F8E8E");

            if (sender == EmailEntry)
            {
                EmailBorder.Stroke = defaultColor;
                EmailBorder.StrokeThickness = 1;
            }
            else if (sender == PasswordInput)
            {
                PasswordBorder.Stroke = defaultColor;
                PasswordBorder.StrokeThickness = 1;
            }
        }

        private void TogglePass_Clicked(object sender, EventArgs e)
        {
            Utility.TogglePasswordVisibility(PasswordInput, ref isPasswordVisible);

            EyeIcon.Source = isPasswordVisible ? "eyeshow.svg" : "eyehidden.svg";
        }

        private void OnSignUpLabelTapped(object sender, EventArgs e)
        {
            Navigation.PushAsync(new SignUpPage());
        }

        private async void OnSignInButtonClicked(object sender, EventArgs e)
        {
            string email = EmailEntry.Text;
            string password = PasswordInput.Text;

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                await DisplayAlert("Error", "Email and Password are required.", "OK");
                return;
            }

            using (var client = new HttpClient())
            {
                try
                {
                    string url = $"https://todo-list.dcism.org/signin_action.php?email={email}&password={password}";
                    var response = await client.GetAsync(url);
                    string responseContent = await response.Content.ReadAsStringAsync();

                    var responseJson = JsonSerializer.Deserialize<SignInResponse>(responseContent);

                    if (responseJson != null && responseJson.status == 200)
                    {
                        await DisplayAlert("Success", responseJson.message, "OK");

                        await Navigation.PushAsync(new ToDoPage(responseJson.data.id));
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
    }

    public class SignInResponse
    {
        public int status { get; set; }
        public SignInData? data { get; set; }
        public string? message { get; set; }
    }

    public class SignInData
    {
        public int id { get; set; }
        public string fname { get; set; }
        public string lname { get; set; }
        public string email { get; set; }
        public string timemodified { get; set; }
    }
}