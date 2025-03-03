namespace signInSignUp.Pages
{
    public partial class SignUpPage : ContentPage
    {
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
    }
}