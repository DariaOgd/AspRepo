using System;

namespace NotesSharingApp.Models.Entities
{
    public class Note
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string NoteBody { get; set; } = string.Empty;
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsPublic { get; set; }
        public string University { get; set; } = string.Empty;
        public Guid? CategoryId { get; set; }
        public Category Category { get; set; }
    }
}
