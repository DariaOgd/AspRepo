namespace NotesSharingApp.Models
{
    public class AddNoteDto
    {
        public string Title { get; set; } = string.Empty;
        public string NoteBody { get; set; } = string.Empty;
        public bool IsPublic { get; set; }
        public string University { get; set; } = string.Empty;
        public Guid CategoryId { get; set; }  // Dodaj to pole
    }
}
