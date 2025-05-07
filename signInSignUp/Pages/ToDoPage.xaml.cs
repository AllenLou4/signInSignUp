using System.Collections.ObjectModel;
using System.Text.Json;
using System.Text;

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

        private async void OnSaveTaskClicked(object sender, EventArgs e)
        {
            string itemName = NewTaskEntry.Text?.Trim();
            string itemDescription = NewDescriptionEntry.Text?.Trim();

            if (string.IsNullOrWhiteSpace(itemName) || string.IsNullOrWhiteSpace(itemDescription))
            {
                await DisplayAlert("Error", "Please fill in both task name and description.", "OK");
                return;
            }

            var newItem = new
            {
                item_name = itemName,
                item_description = itemDescription,
                user_id = 3 // Replace with actual logged-in user ID
            };


            var json = JsonSerializer.Serialize(newItem);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            using var httpClient = new HttpClient();

            try
            {
                var response = await httpClient.PostAsync("https://todo-list.dcism.org/addItem_action.php", content);
                var responseContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine("Response JSON: " + responseContent);

                using var doc = JsonDocument.Parse(responseContent);
                var root = doc.RootElement;

                int status = root.GetProperty("status").GetInt32();
                string message = root.GetProperty("message").GetString();

                if (status == 200)
                {
                    var dataJson = root.GetProperty("data").GetRawText();
                    var task = JsonSerializer.Deserialize<TaskData>(dataJson);  // <-- FIXED HERE

                    Tasks.Add(new TaskItem
                    {
                        ItemId = task.item_id,
                        Name = task.item_name,
                        Description = task.item_description,
                        IsFinished = task.status == "done",
                        UserId = task.user_id,
                        TimeModified = string.IsNullOrWhiteSpace(task.timemodified)
                            ? DateTime.Now
                            : DateTime.Parse(task.timemodified)

                    });

                    NewTaskContainer.IsVisible = false;
                }
                else
                {
                    await DisplayAlert("Error", message ?? "Failed to add task.", "OK");
                }
            }
            catch (Exception ex)  // <-- FIXED HERE
            {
                await DisplayAlert("Error", "An error occurred: " + ex.Message, "OK");
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

        private async void OnCheckBoxCheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            if (sender is CheckBox checkBox && checkBox.BindingContext is TaskItem taskItem)
            {
                if (taskItem.IsFinished)
                {
                    bool isConfirmed = await DisplayAlert("Confirm", "Are you sure you have finished this task?", "Yes", "No");
                    if (isConfirmed)
                    {
                        Tasks.Remove(taskItem);
                        await MoveTaskToFinishedPage(taskItem);
                    }
                    else
                    {
                        taskItem.IsFinished = false;
                        checkBox.IsChecked = false;
                    }
                }
            }
        }

        private async Task MoveTaskToFinishedPage(TaskItem taskItem)
        {
            var finishedPage = new FinishedPage();
            finishedPage.AddTask(new FinishedTaskItem { Name = taskItem.Name, IsFinished = taskItem.IsFinished });
            await Navigation.PushAsync(finishedPage);
        }
    }

    public class TaskItem
    {
        public int ItemId { get; set; } // item_id from response
        public string Name { get; set; } // item_name
        public string Description { get; set; } // optional
        public bool IsFinished { get; set; } // status = "done" or "active"
        public int UserId { get; set; } // needed when posting
        public DateTime TimeModified { get; set; }
    }

    public class AddToDoResponse
    {
        public int status { get; set; }
        public TaskData data { get; set; }
        public string message { get; set; }
    }
    public class TaskData
    {
        public int item_id { get; set; }
        public string item_name { get; set; }
        public string item_description { get; set; }
        public string status { get; set; }
        public int user_id { get; set; }
        public string timemodified { get; set; }
    }

    }
