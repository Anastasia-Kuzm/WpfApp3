using System.Text.RegularExpressions;
using System.Windows;
using WpfApp3.Core;


namespace WpfApp3
{
    public partial class EditNoteWindow : Window
    {
        public Note Result { get; private set; }

        public EditNoteWindow(Note note)
        {
            InitializeComponent();

            TitleBox.Text = note.Title;
            ContentBox.Text = note.Content;
            PreviewText.Text = note.Content;
            TagsBox.Text = note.Tags ?? "";

            // Категория
            var category = note.Category?.Trim();
            if (!string.IsNullOrEmpty(category))
                CategoryBox.SelectedValue = category;
            else
                CategoryBox.SelectedIndex = 4;

            // Приоритет
            PriorityBox.SelectedValue = note.Priority ?? "Средний";

            ContentBox.TextChanged += (s, e) =>
            {
                PreviewText.Text = HighlightText(ContentBox.Text, SearchTerm: null); 
            };

            SaveButton.Click += (s, e) => SaveNote();
            CancelButton.Click += (s, e) => DialogResult = false;
        }

        private void SaveNote()
        {
            var title = TitleBox.Text.Trim();
            var content = ContentBox.Text;
            var tags = TagsBox.Text.Trim();

            // Валидация
            if (string.IsNullOrEmpty(title))
            {
                MessageBox.Show("Заголовок не может быть пустым.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (title.Length > 100)
            {
                MessageBox.Show("Заголовок не должен превышать 100 символов.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (content?.Length > 10000)
            {
                MessageBox.Show("Содержание не должно превышать 10 000 символов.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Валидация тегов
            if (!string.IsNullOrEmpty(tags))
            {
                var cleanTags = Regex.Replace(tags, @"[^a-zA-Zа-яА-Я0-9,_\-\s]", "");
                if (cleanTags != tags)
                {
                    MessageBox.Show("Теги могут содержать только буквы, цифры, дефис, подчёркивание и запятые.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }

            Result = new Note
            {
                Id = Result?.Id ?? 0,
                Title = title,
                Content = content ?? "",
                Category = CategoryBox.SelectedValue?.ToString() ?? "Другое",
                Priority = PriorityBox.SelectedValue?.ToString() ?? "Средний",
                Tags = tags,
            };

            DialogResult = true;
        }

        private string HighlightText(string text, string SearchTerm)
        {
            return text; 
        }
    }
}