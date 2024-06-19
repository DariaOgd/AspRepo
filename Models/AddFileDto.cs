using Microsoft.AspNetCore.Http;

namespace NotesSharingApp.Models
{
    public class AddFileDto
    {
        public IFormFile File { get; set; }
        public string? University { get; set; }
        public string Title { get; set; }
        public Guid? CategoryId { get; set; }
    }
}
