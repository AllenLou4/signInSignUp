using Microsoft.Maui.Controls;
using System.ComponentModel;

namespace signInSignUp.Pages
{
    public partial class ProfilePage : ContentPage, INotifyPropertyChanged
    {
        private readonly int userId;

        public ProfilePage(int userId)
        {
            InitializeComponent();
            this.userId = userId;

            SetActiveTab("profile");

            BindingContext = this;

            NavigationPage.SetHasBackButton(this, false);
            NavigationPage.SetHasNavigationBar(this, false);
        }

        // Bottom nav icon properties
        private string toDoIcon;
        public string ToDoIcon
        {
            get => toDoIcon;
            set
            {
                toDoIcon = value;
                OnPropertyChanged(nameof(ToDoIcon));
            }
        }

        private string finishedIcon;
        public string FinishedIcon
        {
            get => finishedIcon;
            set
            {
                finishedIcon = value;
                OnPropertyChanged(nameof(FinishedIcon));
            }
        }

        private string profileIcon;
        public string ProfileIcon
        {
            get => profileIcon;
            set
            {
                profileIcon = value;
                OnPropertyChanged(nameof(ProfileIcon));
            }
        }

        public Color ToDoBgColor { get; set; }
        public Color FinishedBgColor { get; set; }
        public Color ProfileBgColor { get; set; }

        private void SetActiveTab(string tab)
        {
            ToDoIcon = tab == "todo" ? "list_active.svg" : "list_inactive.svg";
            FinishedIcon = tab == "finished" ? "finished_active.svg" : "finished_inactive.svg";
            ProfileIcon = tab == "profile" ? "profile_active.svg" : "profile_inactive.svg";

            ToDoBgColor = tab == "todo" ? Colors.DarkOliveGreen : Colors.Transparent;
            FinishedBgColor = tab == "finished" ? Colors.DarkOliveGreen : Colors.Transparent;
            ProfileBgColor = tab == "profile" ? Colors.DarkOliveGreen : Colors.Transparent;

            OnPropertyChanged(nameof(ToDoBgColor));
            OnPropertyChanged(nameof(FinishedBgColor));
            OnPropertyChanged(nameof(ProfileBgColor));
        }

        // Navigation handlers
        public async void OnActiveClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ToDoPage(userId, "todo"));
        }

        public async void OnFinishedClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ToDoPage(userId, "finished"));
        }

        public void OnProfileClicked(object sender, EventArgs e)
        {
            // already on this page, do nothing
        }

        public async void OnSignOutClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new MainPage());
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
