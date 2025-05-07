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

        public ToDoPage(int userId)
        {
            InitializeComponent();
            this.userId = userId;

            Tasks = new ObservableCollection<TaskItem>();
            HeaderText = "To Do Today"; 
            BindingContext = this;

            NavigationPage.SetHasBackButton(this, false);
            NavigationPage.SetHasNavigationBar(this, false);

            LoadTasksAsync(currentStatus);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            LoadTasksAsync(currentStatus);
        }

        protected override bool OnBackButtonPressed() => true;

        private async void OnAddTaskClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new TaskDetails(userId));
        }

        private async void OnFinishedClicked(object sender, EventArgs e)
        {
            await LoadTasksAsync("inactive");
        }

        private async void OnActiveClicked(object sender, EventArgs e)
        {
            await LoadTasksAsync("active");
        }

        private async void OnProfileClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ProfilePage());
        }

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

        private async Task LoadTasksAsync(string status = null)
        {
            currentStatus = status ?? currentStatus;
            HeaderText = currentStatus == "active" ? "To Do Today" : "Finished Tasks";

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
