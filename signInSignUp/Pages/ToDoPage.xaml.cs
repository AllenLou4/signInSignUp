using System;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;

namespace signInSignUp.Pages
{
    public partial class ToDoPage : ContentPage
    {
        private readonly string baseUrl = "https://todo-list.dcism.org";
        private readonly int userId; 

        public ObservableCollection<TaskItem> Tasks { get; set; }
        public ToDoPage(int userId)
        {
            InitializeComponent();
            this.userId = userId;

            Tasks = new ObservableCollection<TaskItem>();
            BindingContext = this;

            NavigationPage.SetHasBackButton(this, false);
            NavigationPage.SetHasNavigationBar(this, false);

            LoadTasksAsync(); // Load tasks on start
        }

        protected override bool OnBackButtonPressed() => true;

        private async void OnAddTaskClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new TaskDetails(userId));
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
            await LoadTasksAsync("inactive");
        }

        private async void OnProfileClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ProfilePage());
        }

        private async void OnCheckBoxCheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            // You can later add API code to update task status here
        }

        private async Task LoadTasksAsync(string status = "active")
        {
            try
            {
                using HttpClient client = new();
                string url = $"{baseUrl}/getItems_action.php?status={status}&user_id={userId}";

                var response = await client.GetAsync(url);
                if (!response.IsSuccessStatusCode)
                {
                    await DisplayAlert("Error", "Failed to load tasks", "OK");
                    return;
                }

                string json = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<TaskApiResponse>(json);

                if (result?.Status == 200)
                {
                    Tasks.Clear();
                    foreach (var task in result.Data.Values)
                    {
                        Tasks.Add(new TaskItem
                        {
                            Name = task.ItemName,
                            IsFinished = task.Status == "inactive"
                        });
                    }
                }
                else
                {
                    await DisplayAlert("Error", result?.Message ?? "Unexpected error", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");
            }
        }
    }

    public class TaskItem
    {
        public string Name { get; set; }
        public bool IsFinished { get; set; }
    }

    public class TaskApiResponse
    {
        public int Status { get; set; }
        public string Message { get; set; }
        public Dictionary<string, TaskData> Data { get; set; }
    }

    public class TaskData
    {
        public int ItemId { get; set; }
        public string ItemName { get; set; }
        public string ItemDescription { get; set; }
        public string Status { get; set; }
        public int UserId { get; set; }
        public string Timemodified { get; set; }
    }
}
