namespace signInSignUp.Pages;

public partial class ProfilePage : ContentPage
{
	public ProfilePage()
	{
		InitializeComponent();
	}
    private async void OnSignOutClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new MainPage());
    }
}