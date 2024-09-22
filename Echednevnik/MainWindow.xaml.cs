using System.Collections.ObjectModel;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace Echednevnik
{
   
    public partial class MainWindow : Window
    {
        public class Note
        {
            public string Title { get; set; }
            public string Description { get; set; }
            public DateTime Date { get; set; }

            public Note() { }

            public Note(string title, string description, DateTime date)
            {
                Title = title;
                Description = description;
                Date = date;
            }
        }

        public ObservableCollection<Note> Notes { get; set; } = new ObservableCollection<Note>();
        private Note selectedNote;

        public MainWindow()
        {
            InitializeComponent();
            Date.SelectedDate = DateTime.Today;
            AllNote.ItemsSource = Notes;
            LoadNotes();
        }

        public static class JsonSerializationDeserialization
        {
            public static void Serialize<T>(T data, string filePath)
            {
                string jsonData = JsonConvert.SerializeObject(data);
                File.WriteAllText(filePath, jsonData);
            }

            public static IEnumerable<T> Deserialize<T>(string filePath)
            {
                if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
                {
                    return null;
                }

                try
                {
                    string jsonData = File.ReadAllText(filePath);
                    return JsonConvert.DeserializeObject<IEnumerable<T>>(jsonData);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error while deserializing JSON: " + ex.Message);
                    return null;
                }
            }
        }

        private void SaveNotes()
        {
            JsonSerializationDeserialization.Serialize(Notes, "notes.json");
        }

        private void LoadNotes()
        {
            Notes.Clear();
            IEnumerable<Note> loadedNotes = JsonSerializationDeserialization.Deserialize<Note>("notes.json");
            if (loadedNotes != null)
            {
                foreach (var note in loadedNotes)
                {
                    Notes.Add(note);
                }
                date_SelectedDateChanged(null, null);
            }
        }

        private void date_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            DateTime date = Date.SelectedDate ?? DateTime.Today;
            var filteredNotes = Notes.Where(n => n.Date.Date == date.Date).ToList();
            AllNote.ItemsSource = filteredNotes;
        }

        private void create_note_Click(object sender, RoutedEventArgs e)
        {
            string title = Name.Text.Trim();
            string description = Description.Text.Trim();
            DateTime date = Date.SelectedDate ?? DateTime.Today;

            if (!string.IsNullOrWhiteSpace(title) || !string.IsNullOrWhiteSpace(description))
            {
                Note newNote = new Note(title, description, date);
                Notes.Add(newNote);
                date_SelectedDateChanged(null, null);
                SaveNotes();
            }
        }

        private void delete_note_Click(object sender, RoutedEventArgs e)
        {
            if (selectedNote != null)
            {
                Notes.Remove(selectedNote);
                date_SelectedDateChanged(null, null);
                SaveNotes();
            }
        }

        private void save_note_Click(object sender, RoutedEventArgs e)
        {
            if (selectedNote != null)
            {
                selectedNote.Title = Name.Text;
                selectedNote.Description = Description.Text;
                AllNote.Items.Refresh();
                SaveNotes();
            }
        }

        private void note_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            selectedNote = AllNote.SelectedItem as Note;
            if (selectedNote != null)
            {
                Name.Text = selectedNote.Title;
                Description.Text = selectedNote.Description;
            }
        }
    }
}