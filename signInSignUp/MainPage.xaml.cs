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

        private void TogglePass_Clicked(object sender, EventArgs e)
        {
            Utility.TogglePasswordVisibility(PasswordInput, ref isPasswordVisible);
        }
        private void OnSignUpLabelTapped(object sender, EventArgs e)
        {
            Navigation.PushAsync(new SignUpPage());
        }
        private void OnSignInButtonClicked(object sender, EventArgs e)
        {
            Navigation.PushAsync(new ToDoListPage());
        }
    }
}