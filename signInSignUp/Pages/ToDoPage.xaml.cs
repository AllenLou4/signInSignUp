using System;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using System.Collections.Generic;
using System.ComponentModel;

namespace signInSignUp.Pages
{
    public partial class ToDoPage : ContentPage, INotifyPropertyChanged
    {
        private readonly string baseUrl = "https://todo-list.dcism.org";
        private readonly int userId;
        private int _userId;
        private string _initialTab;
        private string currentStatus = "active";
        public ObservableCollection<TaskItem> Tasks { get; set; }

        private string headerText;
        public string HeaderText
        {
            get => headerText;
            set
            {
                if (headerText != value)
                {
                    headerText = value;
                    OnPropertyChanged(nameof(HeaderText));
                }
            }
        }

        public ToDoPage(int userId, string initialTab = "todo")
        {
            InitializeComponent();
            this.userId = userId;
            _userId = userId;
            _initialTab = initialTab;

            Tasks = new ObservableCollection<TaskItem>();
            HeaderText = "To Do Today"; 
            BindingContext = this;

            SetActiveTab("todo"); // initial state
            LoadTasksAsync("active");

            NavigationPage.SetHasBackButton(this, false);
            NavigationPage.SetHasNavigationBar(this, false);

        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            SetActiveTab(_initialTab == "finished" ? "finished" : "todo");
            await LoadTasksAsync(_initialTab == "finished" ? "inactive" : "active");
        }

        protected override bool OnBackButtonPressed() => true;

        private async void OnAddTaskClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new TaskDetails(userId));
        }

        private async void OnFinishedClicked(object sender, EventArgs e)
        {
            SetActiveTab("finished");
            await LoadTasksAsync("inactive");
        }

        private async void OnActiveClicked(object sender, EventArgs e)
        {
            SetActiveTab("todo");
            await LoadTasksAsync("active");
        }

        private async void OnProfileClicked(object sender, EventArgs e)
        {
            SetActiveTab("profile");
            await Navigation.PushAsync(new ProfilePage(userId));

        }

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

        private void SetActiveTab(string tab)
        {
            ToDoIcon = tab == "todo" ? "list_active.svg" : "list_inactive.svg";
            FinishedIcon = tab == "finished" ? "finished_active.svg" : "finished_inactive.svg";
            ProfileIcon = tab == "profile" ? "profile_active.svg" : "profile_inactive.svg";

            ToDoBgColor = tab == "todo" ? Colors.DarkOliveGreen : Colors.Transparent;
            FinishedBgColor = tab == "finished" ? Colors.DarkOliveGreen : Colors.Transparent;
            ProfileBgColor = tab == "profile" ? Colors.DarkOliveGreen : Colors.Transparent;

            // Notify UI
            OnPropertyChanged(nameof(ToDoBgColor));
            OnPropertyChanged(nameof(FinishedBgColor));
            OnPropertyChanged(nameof(ProfileBgColor));
        }

        public Color ToDoBgColor { get; set; }
        public Color FinishedBgColor { get; set; }
        public Color ProfileBgColor { get; set; }

        private async void OnCheckBoxCheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            if (sender is CheckBox checkBox && checkBox.BindingContext is TaskItem task)
            {
                string newStatus = e.Value ? "inactive" : "active";

                var data = new
                {
                    status = newStatus,
                    item_id = task.ItemId
                };

                var json = JsonSerializer.Serialize(data);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                using HttpClient client = new();
                var request = new HttpRequestMessage(HttpMethod.Put, $"{baseUrl}/statusItem_action.php")
                {
                    Content = content
                };

                try
                {
                    var response = await client.SendAsync(request);
                    string resultJson = await response.Content.ReadAsStringAsync();
                    var result = JsonSerializer.Deserialize<ApiResponse>(resultJson);

                    if (result?.Status == 200)
                    {
                        await LoadTasksAsync(currentStatus);
                    }
                    else
                    {
                        await DisplayAlert("Error", result?.Message ?? "Failed to update status", "OK");
                    }
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Error", ex.Message, "OK");
                }
            }
        }

        private async void OnTaskTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item is TaskItem selectedTask)
            {
                await Navigation.PushAsync(new TaskDetails(userId, selectedTask));
                ((ListView)sender).SelectedItem = null; // Deselect item
            }
        }


        private async Task LoadTasksAsync(string status = null)
        {
            currentStatus = status ?? currentStatus;
            HeaderText = currentStatus == "active" ? "To Do Today" : "Completed Tasks";

            try
            {
                using HttpClient client = new();
                string url = $"{baseUrl}/getItems_action.php?status={currentStatus}&user_id={userId}";

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
                            ItemId = task.ItemId,
                            Name = task.ItemName,
                            Description = task.ItemDescription,
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

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public class TaskItem
    {
        public int ItemId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsFinished { get; set; }
    }

    public class TaskApiResponse
    {
        [JsonPropertyName("status")]
        public int Status { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; }

        [JsonPropertyName("data")]
        public Dictionary<string, TaskData> Data { get; set; }
    }

    public class TaskData
    {
        [JsonPropertyName("item_id")]
        public int ItemId { get; set; }

        [JsonPropertyName("item_name")]
        public string ItemName { get; set; }

        [JsonPropertyName("item_description")]
        public string ItemDescription { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; }

        [JsonPropertyName("user_id")]
        public int UserId { get; set; }

        [JsonPropertyName("timemodified")]
        public string Timemodified { get; set; }
    }

    public class ApiResponse
    {
        [JsonPropertyName("status")]
        public int Status { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; }
    }
}
