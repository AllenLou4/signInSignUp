namespace signInSignUp.Pages
{
    using signInSignUp.Helpers;
    public partial class SignUpPage : ContentPage
    {
        bool isPasswordVisible = false;

        public SignUpPage()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
        }

        private void OnSignUpButtonClicked(object sender, EventArgs e)
        {
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