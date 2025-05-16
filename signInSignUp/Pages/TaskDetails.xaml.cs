using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using System.Net.Http;
using System.Text;

namespace signInSignUp.Pages
{
    public partial class TaskDetails : ContentPage
    {
        private readonly int userId;
        private bool isNewTask;
        private TaskItem TaskItem { get; set; }
        public TaskDetails(int userId, TaskItem task)
        {
            InitializeComponent();
            this.userId = userId;
            this.TaskItem = task;
            this.isNewTask = task == null;

            if (isNewTask)
            {
                AddButton.IsVisible = true;
            }
            else
            {
                TaskNameEntry.Text = task.Name;
                TaskDescriptionEditor.Text = task.Description;
            }
        }

        public TaskDetails(int userId) : this(userId, null) { }


        // Navigate back
        private async void OnBackClicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }

        private async Task AnimateButtonPress(ImageButton button)
        {
            button.IsEnabled = false;
            var originalColor = button.BackgroundColor;
            button.BackgroundColor = Colors.LightGray;
            await button.ScaleTo(0.9, 50);
            await button.ScaleTo(1.0, 50);
            button.BackgroundColor = originalColor;
            button.IsEnabled = true;
        }

        private async Task AnimateBackgroundFlash(ImageButton button, Color flashColor)
        {
            var originalColor = button.BackgroundColor;
            button.BackgroundColor = flashColor;
            await Task.Delay(100);
            button.BackgroundColor = originalColor;
        }

        // Attach file handler
        private async void OnAttachClicked(object sender, EventArgs e)
        {
            await AnimateButtonPress((ImageButton)sender);
            /*
            try
            {
                var result = await FilePicker.PickAsync(new PickOptions
                {
                    PickerTitle = "Select a file"
                });

                if (result != null)
                {
                    FileNameLabel.Text = result.FileName;
                    FileIcon.Source = "file_icon.png"; // Replace with your icon name
                    AttachmentPreview.IsVisible = true;

                    StatusLabel.Text = "File attached successfully.";
                    StatusLabel.TextColor = Colors.Green;
                    StatusLabel.IsVisible = true;
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"File selection failed: {ex.Message}", "OK");
            }
            */
        }

        // Attach image handler
        private async void OnImageClicked(object sender, EventArgs e)
        {
            await AnimateButtonPress((ImageButton)sender);
            /*
            try
            {
                var result = await FilePicker.PickAsync(new PickOptions
                {
                    PickerTitle = "Select an image",
                    FileTypes = FilePickerFileType.Images
                });

                if (result != null)
                {
                    using var stream = await result.OpenReadAsync();
                    SelectedImage.Source = ImageSource.FromStream(() => stream);
                    ImagePreview.IsVisible = true;

                    StatusLabel.Text = "Image attached successfully.";
                    StatusLabel.TextColor = Colors.Green;
                    StatusLabel.IsVisible = true;
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Image selection failed: {ex.Message}", "OK");
            }
            */
        }

        // 3-dot menu handler (placeholder)
        private async void OnMoreOptionsClicked(object sender, EventArgs e)
        {
            await AnimateButtonPress((ImageButton)sender);

            string action = await DisplayActionSheet("Options", "Cancel", null, "SAVE TASK", "DELETE TASK");
            switch (action)
            {
                case "SAVE TASK":
                    await SaveTaskAsync();
                    break;
                case "DELETE TASK":
                    await DeleteTaskAsync();
                    break;
            }
        }

        private async Task SaveTaskAsync()
        {
            var updatedName = TaskNameEntry.Text?.Trim();
            var updatedDescription = TaskDescriptionEditor.Text?.Trim();

            if (string.IsNullOrEmpty(updatedName))
            {
                await DisplayAlert("Validation Error", "Task name cannot be empty.", "OK");
                return;
            }

            var client = new HttpClient();
            var requestBody = new
            {
                item_name = updatedName,
                item_description = updatedDescription,
                user_id = userId
            };

            var content = new StringContent(
                System.Text.Json.JsonSerializer.Serialize(requestBody),
                System.Text.Encoding.UTF8,
                "application/json");

            var response = await client.PutAsync($"https://todo-list.dcism.org/editItem_action.php?item_id={TaskItem.ItemId}", content);

            if (response.IsSuccessStatusCode)
            {
                await DisplayAlert("Success", "Task updated successfully.", "OK");
                await Navigation.PopAsync();
            }
            else
            {
                await DisplayAlert("Error", "Failed to update task.", "OK");
            }
        }

        private async Task DeleteTaskAsync()
        {
            var confirm = await DisplayAlert("Confirm", "Are you sure you want to delete this task?", "Yes", "No");
            if (!confirm) return;

            var client = new HttpClient();
            var response = await client.DeleteAsync($"https://todo-list.dcism.org/deleteItem_action.php?item_id={TaskItem.ItemId}");

            if (response.IsSuccessStatusCode)
            {
                await DisplayAlert("Deleted", "Task deleted successfully.", "OK");
                await Navigation.PopAsync();
            }
            else
            {
                await DisplayAlert("Error", "Failed to delete task.", "OK");
            }
        }

        private async void OnAddTaskClicked(object sender, EventArgs e)
        {
            string taskName = TaskNameEntry.Text?.Trim();
            string taskDescription = TaskDescriptionEditor.Text?.Trim();

            if (string.IsNullOrWhiteSpace(taskName) || string.IsNullOrWhiteSpace(taskDescription))
            {
                await DisplayAlert("Error", "Please enter both a task name and description.", "OK");
                return;
            }

            var newTask = new
            {
                item_name = taskName,
                item_description = taskDescription,
                user_id = userId
            };

            using var client = new HttpClient();
            var content = new StringContent(JsonSerializer.Serialize(newTask), Encoding.UTF8, "application/json");

            try
            {
                var response = await client.PostAsync("https://todo-list.dcism.org/addItem_action.php", content);
                var responseContent = await response.Content.ReadAsStringAsync();

                var result = JsonSerializer.Deserialize<AddTaskResponse>(responseContent);

                if (result != null && result.status == 200)
                {
                    StatusLabel.Text = "Task added successfully!";
                    StatusLabel.IsVisible = true;
                    await Task.Delay(1000);
                    await Navigation.PopAsync();
                }
                else
                {
                    await DisplayAlert("Error", result?.message ?? "An error occurred.", "OK");
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to connect: {ex.Message}", "OK");
            }
        }

        public class AddTaskResponse
        {
            public int status { get; set; }
            public string message { get; set; }
            public TaskData data { get; set; }
        }

        public class TaskData
        {
            public int item_id { get; set; }
            public string item_name { get; set; }
            public string item_description { get; set; }
            public int user_id { get; set; }
        }

    }
}
