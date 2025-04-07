using System.Collections.ObjectModel;

namespace signInSignUp.Pages
{
    public partial class ToDoListPage : ContentPage
    {
        public ObservableCollection<TaskItem> Tasks { get; set; }

        public ToDoListPage()
        {
            InitializeComponent();
            Tasks = new ObservableCollection<TaskItem>();
            BindingContext = this;

            NavigationPage.SetHasBackButton(this, false);
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
    }

    public class TaskItem
    {
        public string Name { get; set; }
        public bool IsFinished { get; set; }
    }
}