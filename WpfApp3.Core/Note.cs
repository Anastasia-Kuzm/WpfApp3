using System;

namespace WpfApp3.Core
{
    public class Note
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string Category { get; set; }
        public string Priority { get; set; } 
        public string Tags { get; set; }    
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
        public bool IsPinned { get; set; }

        public Note()
        {
            CreatedDate = DateTime.Now;
            ModifiedDate = DateTime.Now;
            Priority = "Средний";
            Tags = "";
        }
    }
}