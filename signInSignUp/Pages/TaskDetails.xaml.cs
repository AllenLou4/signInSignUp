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
            public string status { get; set; }
            public int user_id { get; set; }
            public string timemodified { get; set; }
        }
    }
}
