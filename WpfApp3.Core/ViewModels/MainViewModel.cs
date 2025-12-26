using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using WpfApp3.Core;

namespace WpfApp3.Core.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly INoteRepository _repo;
        private Note _selectedNote;
        private string _searchQuery = "";
        private string _currentCategory = "Все заметки";

        public ObservableCollection<string> Categories { get; }
        public ObservableCollection<Note> Notes { get; }

        public string SearchQuery
        {
            get => _searchQuery;
            set { _searchQuery = value; OnPropertyChanged(); FilterNotes(); }
        }

        public string CurrentCategory
        {
            get => _currentCategory;
            set { _currentCategory = value; OnPropertyChanged(); FilterNotes(); }
        }

        public Note SelectedNote
        {
            get => _selectedNote;
            set { _selectedNote = value; OnPropertyChanged(); UpdateCommands(); }
        }

        public ICommand NewNoteCommand { get; }
        public ICommand EditNoteCommand { get; }
        public ICommand DeleteNoteCommand { get; }
        public ICommand PinNoteCommand { get; }

        private void UpdateCommands()
        {
            ((RelayCommand)EditNoteCommand).RaiseCanExecuteChanged();
            ((RelayCommand)DeleteNoteCommand).RaiseCanExecuteChanged();
            ((RelayCommand)PinNoteCommand).RaiseCanExecuteChanged();
        }

        public MainViewModel(INoteRepository repo)
        {
            _repo = repo;
            Categories = new ObservableCollection<string> { "Все заметки", "Работа", "Личное", "Идеи", "Покупки", "Другое" };
            Notes = new ObservableCollection<Note>();
            LoadNotes();

            NewNoteCommand = new RelayCommand(_ => NewNote());
            EditNoteCommand = new RelayCommand(_ => EditNote(), _ => SelectedNote != null);
            DeleteNoteCommand = new RelayCommand(_ => DeleteNote(), _ => SelectedNote != null);
            PinNoteCommand = new RelayCommand(_ => TogglePin(), _ => SelectedNote != null);
        }

        private void LoadNotes()
        {
            var all = _repo.GetAllNotes();
            Notes.Clear();
            foreach (var note in all) Notes.Add(note);
            FilterNotes();
        }

        private void FilterNotes()
        {
            var all = _repo.GetAllNotes();

            if (CurrentCategory != "Все заметки")
                all = all.Where(n => n.Category == CurrentCategory).ToList();

            if (!string.IsNullOrWhiteSpace(SearchQuery))
            {
                var q = SearchQuery.ToLower();
                all = all.Where(n =>
                    (n.Title?.ToLower().Contains(q) == true) ||
                    (n.Content?.ToLower().Contains(q) == true)
                ).ToList();
            }

            Notes.Clear();
            foreach (var note in all) Notes.Add(note);
        }

        private void NewNote() { }
        private void EditNote() { }

        private void DeleteNote()
        {
            if (SelectedNote != null)
            {
                _repo.DeleteNote(SelectedNote.Id);
                LoadNotes();
                SelectedNote = null;
            }
        }

        private void TogglePin()
        {
            if (SelectedNote != null)
            {
                _repo.TogglePin(SelectedNote.Id);
                LoadNotes();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}