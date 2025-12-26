using System;
using System.Collections.Generic;

namespace WpfApp3.Core
{
    public interface INoteRepository
    {
        List<Note> GetAllNotes();
        List<Note> SearchNotes(string query);
        List<Note> FilterByCategory(string category);
        void AddNote(Note note);
        void UpdateNote(Note note);
        void DeleteNote(int id);
        void TogglePin(int id);
    }
}