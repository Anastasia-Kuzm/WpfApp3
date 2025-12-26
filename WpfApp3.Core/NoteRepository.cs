using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using WpfApp3.Core;

namespace WpfApp3.Core
{
    public class NoteRepository : INoteRepository
    {
        private readonly string _connectionString = "Data Source=notes.db";

        public NoteRepository()
        {
            EnsureDatabaseCreated();
        }

        private void EnsureDatabaseCreated()
        {
            if (!File.Exists("notes.db"))
                SQLiteConnection.CreateFile("notes.db");

            using (var conn = new SQLiteConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = new SQLiteCommand(@"
                    CREATE TABLE IF NOT EXISTS Notes (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Title TEXT NOT NULL,
                        Content TEXT,
                        Category TEXT,
                        Priority TEXT,
                        Tags TEXT,
                        CreatedDate TEXT,
                        ModifiedDate TEXT,
                        IsPinned INTEGER
                    )", conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public List<Note> GetAllNotes()
        {
            var notes = new List<Note>();
            using (var conn = new SQLiteConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = new SQLiteCommand("SELECT * FROM Notes ORDER BY IsPinned DESC, ModifiedDate DESC", conn))
                using (var reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        notes.Add(new Note
                        {
                            Id = reader.GetInt32(0),
                            Title = reader.GetString(1),
                            Content = reader.IsDBNull(2) ? null : reader.GetString(2),
                            Category = reader.IsDBNull(3) ? null : reader.GetString(3),
                            Priority = reader.IsDBNull(4) ? null : reader.GetString(4),
                            Tags = reader.IsDBNull(5) ? null : reader.GetString(5),
                            CreatedDate = DateTime.Parse(reader.GetString(6)),
                            ModifiedDate = DateTime.Parse(reader.GetString(7)),
                            IsPinned = reader.GetInt32(8) != 0
                        });
                    }
                }
            }
            return notes;
        }

        public List<Note> SearchNotes(string query)
        {
            if (string.IsNullOrWhiteSpace(query)) return GetAllNotes();

            var notes = new List<Note>();
            using (var conn = new SQLiteConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = new SQLiteCommand(@"
                    SELECT * FROM Notes 
                    WHERE Title LIKE @q OR Content LIKE @q OR Tags LIKE @q
                    ORDER BY IsPinned DESC, ModifiedDate DESC", conn))
                {
                    cmd.Parameters.AddWithValue("@q", "%" + query + "%");
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            notes.Add(new Note
                            {
                                Id = reader.GetInt32(0),
                                Title = reader.GetString(1),
                                Content = reader.IsDBNull(2) ? null : reader.GetString(2),
                                Category = reader.IsDBNull(3) ? null : reader.GetString(3),
                                Priority = reader.IsDBNull(4) ? null : reader.GetString(4),
                                Tags = reader.IsDBNull(5) ? null : reader.GetString(5),
                                CreatedDate = DateTime.Parse(reader.GetString(6)),
                                ModifiedDate = DateTime.Parse(reader.GetString(7)),
                                IsPinned = reader.GetInt32(8) != 0
                            });
                        }
                    }
                }
            }
            return notes;
        }

        public List<Note> FilterByCategory(string category)
        {
            if (string.IsNullOrEmpty(category) || category == "Все заметки")
                return GetAllNotes();

            var notes = new List<Note>();
            using (var conn = new SQLiteConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = new SQLiteCommand(@"
                    SELECT * FROM Notes 
                    WHERE Category = @cat
                    ORDER BY IsPinned DESC, ModifiedDate DESC", conn))
                {
                    cmd.Parameters.AddWithValue("@cat", category);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            notes.Add(new Note
                            {
                                Id = reader.GetInt32(0),
                                Title = reader.GetString(1),
                                Content = reader.IsDBNull(2) ? null : reader.GetString(2),
                                Category = reader.IsDBNull(3) ? null : reader.GetString(3),
                                Priority = reader.IsDBNull(4) ? null : reader.GetString(4),
                                Tags = reader.IsDBNull(5) ? null : reader.GetString(5),
                                CreatedDate = DateTime.Parse(reader.GetString(6)),
                                ModifiedDate = DateTime.Parse(reader.GetString(7)),
                                IsPinned = reader.GetInt32(8) != 0
                            });
                        }
                    }
                }
            }
            return notes;
        }

        public void AddNote(Note note)
        {
            note.CreatedDate = DateTime.Now;
            note.ModifiedDate = DateTime.Now;
            using (var conn = new SQLiteConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = new SQLiteCommand(@"
                    INSERT INTO Notes (Title, Content, Category, Priority, Tags, CreatedDate, ModifiedDate, IsPinned)
                    VALUES (@t, @c, @cat, @p, @tg, @cd, @md, @pin)", conn))
                {
                    cmd.Parameters.AddWithValue("@t", note.Title);
                    cmd.Parameters.AddWithValue("@c", note.Content ?? "");
                    cmd.Parameters.AddWithValue("@cat", note.Category ?? "");
                    cmd.Parameters.AddWithValue("@p", note.Priority ?? "");
                    cmd.Parameters.AddWithValue("@tg", note.Tags ?? "");
                    cmd.Parameters.AddWithValue("@cd", note.CreatedDate.ToString("O"));
                    cmd.Parameters.AddWithValue("@md", note.ModifiedDate.ToString("O"));
                    cmd.Parameters.AddWithValue("@pin", note.IsPinned ? 1 : 0);
                    cmd.ExecuteNonQuery();
                    note.Id = (int)conn.LastInsertRowId;
                }
            }
        }

        public void UpdateNote(Note note)
        {
            note.ModifiedDate = DateTime.Now;
            using (var conn = new SQLiteConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = new SQLiteCommand(@"
                    UPDATE Notes SET Title=@t, Content=@c, Category=@cat, Priority=@p, Tags=@tg, ModifiedDate=@md, IsPinned=@pin
                    WHERE Id=@id", conn))
                {
                    cmd.Parameters.AddWithValue("@id", note.Id);
                    cmd.Parameters.AddWithValue("@t", note.Title);
                    cmd.Parameters.AddWithValue("@c", note.Content ?? "");
                    cmd.Parameters.AddWithValue("@cat", note.Category ?? "");
                    cmd.Parameters.AddWithValue("@p", note.Priority ?? "");
                    cmd.Parameters.AddWithValue("@tg", note.Tags ?? "");
                    cmd.Parameters.AddWithValue("@md", note.ModifiedDate.ToString("O"));
                    cmd.Parameters.AddWithValue("@pin", note.IsPinned ? 1 : 0);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void DeleteNote(int id)
        {
            using (var conn = new SQLiteConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = new SQLiteCommand("DELETE FROM Notes WHERE Id=@id", conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void TogglePin(int id)
        {
            using (var conn = new SQLiteConnection(_connectionString))
            {
                conn.Open();
                using (var cmd = new SQLiteCommand("UPDATE Notes SET IsPinned = 1 - IsPinned WHERE Id=@id", conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}