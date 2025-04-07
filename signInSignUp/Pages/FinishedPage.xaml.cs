namespace signInSignUp.Pages;

public partial class FinishedPage : ContentPage
{
    public FinishedPage(){
        InitializeComponent();
        NavigationPage.SetHasBackButton(this, false);
    }
    private async void OnToDoTodayClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new ToDoListPage());
    }
    private async void OnProfileClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new ProfilePage());
    }
}
