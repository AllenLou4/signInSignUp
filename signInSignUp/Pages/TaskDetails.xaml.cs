using Microsoft.Maui.Controls;

namespace signInSignUp.Pages
{
    public partial class TaskDetails : ContentPage
    {
        private string _taskId;
        private string _existingNote;

        public TaskDetails(string taskId, string existingNote = "")
        {
            InitializeComponent();

            _taskId = taskId;
            _existingNote = existingNote;

            NotesEditor.Text = _existingNote;
        }

        // example save method
        private async void SaveNoteButton_Clicked(object sender, EventArgs e)
        {
            string updatedNote = NotesEditor.Text;

            await DisplayAlert("Saved", "Your notes have been updated.", "OK");

            await Navigation.PopAsync();
        }
    }
}