using System.Collections.ObjectModel;

namespace signInSignUp.Pages
{
    public partial class FinishedPage : ContentPage
    {
        public ObservableCollection<FinishedTaskItem> FinishedTasks { get; set; }

        public FinishedPage()
        {
            InitializeComponent();
            FinishedTasks = new ObservableCollection<FinishedTaskItem>
            {
                new FinishedTaskItem { Name = "Buy groceries", IsFinished = true },
                new FinishedTaskItem { Name = "Walk the dog", IsFinished = true },
                new FinishedTaskItem { Name = "Finish project report", IsFinished = true },
                new FinishedTaskItem { Name = "Call mom", IsFinished = true },
                new FinishedTaskItem { Name = "Read a book", IsFinished = true }
            };
            BindingContext = this;

            NavigationPage.SetHasBackButton(this, false);
            NavigationPage.SetHasNavigationBar(this, false);
        }

        protected override bool OnBackButtonPressed()
        {
            return true;
        }

        private async void OnToDoTodayClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ToDoListPage());
        }

        private async void OnProfileClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ProfilePage());
        }

        public void AddTask(FinishedTaskItem taskItem)
        {
            FinishedTasks.Add(taskItem);
        }
    }

    public class FinishedTaskItem
    {
        public string Name { get; set; }
        public bool IsFinished { get; set; }
    }
}