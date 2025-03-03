namespace signInSignUp
{
    public partial class MainPage : ContentPage
    {
        int count = 0;
        bool isPasswordVisible = false;

        public MainPage()
        {
            InitializeComponent();
            NavigationPage.SetHasNavigationBar(this, false);
        }

        private void TogglePass_Clicked(object sender, EventArgs e)
        {
            isPasswordVisible = !isPasswordVisible;
            PasswordInput.IsPassword = !isPasswordVisible;
            TogglePass.Text = isPasswordVisible ? "Hide" : "Show";
        }
    }
}