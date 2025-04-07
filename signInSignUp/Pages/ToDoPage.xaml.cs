using System.Collections.ObjectModel;

namespace signInSignUp.Pages
{
    public partial class ToDoListPage : ContentPage
    {
        public ObservableCollection<TaskItem> Tasks { get; set; }

        public ToDoListPage()
        {
            InitializeComponent();
            Tasks = new ObservableCollection<TaskItem>
            {
                new TaskItem { Name = "Buy groceries", IsFinished = false },
                new TaskItem { Name = "Walk the dog", IsFinished = false },
                new TaskItem { Name = "Finish project report", IsFinished = false },
                new TaskItem { Name = "Call mom", IsFinished = false }
            };
            BindingContext = this;

            NavigationPage.SetHasBackButton(this, false);
            NavigationPage.SetHasNavigationBar(this, false);
        }

        protected override bool OnBackButtonPressed()
        {
            return true;
        }

        private void OnAddTaskClicked(object sender, EventArgs e)
        {
            NewTaskContainer.IsVisible = true;
            NewTaskEntry.Text = string.Empty;
        }

        private void OnSaveTaskClicked(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(NewTaskEntry.Text))
            {
                Tasks.Add(new TaskItem { Name = NewTaskEntry.Text, IsFinished = false });
                NewTaskContainer.IsVisible = false;
            }
        }

        private void OnCancelTaskClicked(object sender, EventArgs e)
        {
            NewTaskContainer.IsVisible = false;
        }

        private async void OnFinishedClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new FinishedPage());
        }

        private async void OnProfileClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ProfilePage());
        }
    }

    public class TaskItem
    {
        public string Name { get; set; }
        public bool IsFinished { get; set; }
    }
}