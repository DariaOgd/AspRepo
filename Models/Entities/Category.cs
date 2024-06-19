using System;
using System.Collections.Generic;

namespace NotesSharingApp.Models.Entities
{
    public class Category
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;

        public ICollection<Note> Notes { get; set; } = new List<Note>();
        public ICollection<File> Files { get; set; } = new List<File>();
    }
}
