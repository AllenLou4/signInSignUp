using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace signInSignUp.Pages
{
    public partial class TaskDetails : ContentPage
    {
        private int userId;

        public TaskDetails(int userId)
        {
            InitializeComponent();
            this.userId = userId;
        }

        private void OnBackClicked(object sender, EventArgs e)
        {
            Navigation.PopAsync();
        }

        private async void OnAttachClicked(object sender, EventArgs e)
        {
            try
            {
                var result = await FilePicker.Default.PickAsync(new PickOptions
                {
                    PickerTitle = "Select a file"
                });

                if (result != null)
                {
                    string fileName = result.FileName;
                    string fileExtension = Path.GetExtension(fileName).ToLower();

                    // Choose an icon based on the file type
                    string iconSource = fileExtension switch
                    {
                        ".pdf" => "pdf_icon.png",
                        ".doc" or ".docx" => "doc_icon.png",
                        ".xls" or ".xlsx" => "excel_icon.png",
                        ".ppt" or ".pptx" => "ppt_icon.png",
                        ".txt" => "text_icon.png",
                        _ => "file_icon.png"
                    };

                    FileIcon.Source = iconSource;
                    FileNameLabel.Text = fileName;
                    AttachmentPreview.IsVisible = true;

                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"File selection failed: {ex.Message}", "OK");
            }
        }

        private async void OnImageClicked(object sender, EventArgs e)
        {
            try
            {
                var result = await MediaPicker.PickPhotoAsync(new MediaPickerOptions
                {
                    Title = "Select an image"
                });

                if (result != null)
                {
                    using var stream = await result.OpenReadAsync();
                    SelectedImage.Source = ImageSource.FromStream(() => stream);
                    ImagePreview.IsVisible = true;
                }
            }
            catch (FeatureNotSupportedException)
            {
                await DisplayAlert("Error", "Photo picking is not supported on this device.", "OK");
            }
            catch (PermissionException)
            {
                await DisplayAlert("Error", "Permission to access photos was denied.", "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Image selection failed: {ex.Message}", "OK");
            }
        }

        private async void OnMoreOptionsClicked(object sender, EventArgs e)
        {
            string action = await DisplayActionSheet("Options", "Cancel", null, "Save", "Delete");

            switch (action)
            {
                case "Save":
                    OnSaveTaskClicked(sender, e);
                    break;

                case "Delete":
                    bool confirm = await DisplayAlert("Confirm Delete", "Are you sure you want to delete this task?", "Yes", "No");
                    if (confirm)
                    {
                        await DisplayAlert("Deleted", "Task has been deleted.", "OK"); // Replace with actual delete logic
                        await Navigation.PopAsync(); // Navigate back
                    }
                    break;
            }
        }

        private async void OnSaveTaskClicked(object sender, EventArgs e)
        {
            string taskName = TaskNameEntry.Text;
            string taskDescription = TaskDescriptionEditor.Text;

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

                    await Task.Delay(900);
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

        private async Task DeleteTaskAsync()
        {
            // Call delete API here if task has ID
            await DisplayAlert("Deleted", "Task has been deleted.", "OK");
            await Navigation.PopAsync();
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
            public string status { get; set; }
            public int user_id { get; set; }
            public string timemodified { get; set; }
        }
    }
}
