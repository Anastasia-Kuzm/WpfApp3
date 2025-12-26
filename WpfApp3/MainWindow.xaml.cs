using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using WpfApp3.Core;

namespace WpfApp3
{
    public partial class MainWindow : Window
    {
        private readonly NoteRepository _repo = new NoteRepository();
        private Note _selectedNote;
        private string _currentCategory = "Все заметки";
        private string _filterPriority = "Все"; 
        private string _filterTag = "";         

        public MainWindow()
        {
            InitializeComponent();
            LoadCategories();
            LoadNotes();
            WireEvents();
        }

        private void WireEvents()
        {
            NewButton.Click += (s, e) => NewNote();
            EditButton.Click += (s, e) => EditNote();
            DeleteButton.Click += (s, e) => DeleteNote();
            PinButton.Click += (s, e) => TogglePin();
        }

        private void LoadCategories()
        {
            var categories = new List<string> { "Все заметки", "Работа", "Личное", "Идеи", "Покупки", "Другое" };
            CategoriesList.ItemsSource = categories;

            for (int i = 0; i < CategoriesList.Items.Count; i++)
            {
                if (CategoriesList.Items[i].ToString() == _currentCategory)
                {
                    CategoriesList.SelectedIndex = i;
                    break;
                }
            }
        }

        private void LoadNotes(string query = null)
        {
            var allNotes = _repo.GetAllNotes();

            // Фильтр по категории
            if (_currentCategory != "Все заметки")
                allNotes = allNotes.Where(n => n.Category == _currentCategory).ToList();

            // Фильтр по поиску 
            if (!string.IsNullOrWhiteSpace(query))
            {
                query = query.Trim().ToLower();
                allNotes = allNotes.Where(n =>
                    (!string.IsNullOrEmpty(n.Title) && n.Title.ToLower().Contains(query)) ||
                    (!string.IsNullOrEmpty(n.Content) && n.Content.ToLower().Contains(query)) ||
                    (!string.IsNullOrEmpty(n.Tags) && n.Tags.ToLower().Contains(query))
                ).ToList();
            }

            // Фильтр по приоритету
            if (_filterPriority != "Все")
                allNotes = allNotes.Where(n => n.Priority == _filterPriority).ToList();

            // Фильтр по тегу
            if (!string.IsNullOrEmpty(_filterTag))
            {
                var tagLower = _filterTag.Trim().ToLower();
                allNotes = allNotes.Where(n =>
                    !string.IsNullOrEmpty(n.Tags) &&
                    n.Tags.Split(',')
                        .Select(t => t.Trim())
                        .Any(t => t.ToLower() == tagLower)
                ).ToList();
            }

            NotesList.ItemsSource = allNotes;
        }

        private void NewNote()
        {
            var win = new EditNoteWindow(new Note());
            if (win.ShowDialog() == true)
            {
                _repo.AddNote(win.Result);
                LoadNotes(SearchBox.Text);
            }
        }

        private void EditNote()
        {
            if (_selectedNote == null) return;
            var win = new EditNoteWindow(_selectedNote);
            if (win.ShowDialog() == true)
            {
                win.Result.Id = _selectedNote.Id;
                _repo.UpdateNote(win.Result);
                LoadNotes(SearchBox.Text);
            }
        }

        private void DeleteNote()
        {
            if (_selectedNote == null) return;
            _repo.DeleteNote(_selectedNote.Id);
            LoadNotes(SearchBox.Text);
            _selectedNote = null;
            UpdateButtonState();
        }

        private void TogglePin()
        {
            if (_selectedNote == null) return;
            _repo.TogglePin(_selectedNote.Id);
            LoadNotes(SearchBox.Text);
        }

        private void NotesList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _selectedNote = NotesList.SelectedItem as Note;
            UpdateButtonState();
        }

        private void CategoriesList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CategoriesList.SelectedItem != null)
            {
                _currentCategory = CategoriesList.SelectedItem.ToString();
                LoadNotes(SearchBox.Text);
            }
        }

        private void SearchBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            Placeholder.Visibility = string.IsNullOrWhiteSpace(SearchBox.Text)
                ? Visibility.Visible
                : Visibility.Collapsed;
            LoadNotes(SearchBox.Text);
        }

        private void FilterPriority_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ComboBox cb && cb.SelectedItem is ComboBoxItem item)
            {
                _filterPriority = item.Content.ToString();
                LoadNotes(SearchBox.Text);
            }
        }

        private void FilterTag_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox tb)
            {
                _filterTag = tb.Text;
                LoadNotes(SearchBox.Text);
            }
        }

        private void UpdateButtonState()
        {
            bool enabled = _selectedNote != null;
            EditButton.IsEnabled = enabled;
            DeleteButton.IsEnabled = enabled;
            PinButton.IsEnabled = enabled;
        }
    }
}