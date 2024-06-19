using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace NotesSharingApp.Models.Entities
{
    public class File
    {
        public Guid Id { get; set; }
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public byte[] Data { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        // Dodaj pole University
        [Column(TypeName = "nvarchar(max)")]
        public string? University { get; set; }

        // Dodaj pole Title
        public string? Title { get; set; }

        // Dodaj pole CategoryId
        public Guid? CategoryId { get; set; }
        public Category Category { get; set; }
    }
}
