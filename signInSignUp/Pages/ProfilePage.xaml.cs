namespace signInSignUp.Pages;

public partial class ProfilePage : ContentPage
{
	public ProfilePage()
	{
		InitializeComponent();
        NavigationPage.SetHasBackButton(this, false);
        NavigationPage.SetHasNavigationBar(this, false);
    }
    protected override bool OnBackButtonPressed(){
        return true;
    }
    private async void OnSignOutClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new MainPage());
    }
    private async void OnToDoTodayClicked(object sender, EventArgs e)
    {
        //await Navigation.PushAsync(new ToDoPage());
    }
    private async void OnFinishedClicked(object sender, EventArgs e)
    {
        //await Navigation.PushAsync(new FinishedPage());
    }
}