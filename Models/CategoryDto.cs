using System;
using System.Collections.Generic;

namespace NotesSharingApp.Models
{
    public class CategoryDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<NoteDto> Notes { get; set; } = new List<NoteDto>();
        public List<FileDto> Files { get; set; } = new List<FileDto>(); // Dodaj to pole
    }

    public class NoteDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string NoteBody { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public bool IsPublic { get; set; }
        public string University { get; set; } = string.Empty;
        public Guid? CategoryId { get; set; } // Dodaj to pole
    }

    public class FileDto // Dodaj tę nową klasę
    {
        public Guid Id { get; set; }
        public string FileName { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty; // Dodaj to pole
        public string? University { get; set; }
        public Guid? CategoryId { get; set; } // Dodaj to pole
    }
}
