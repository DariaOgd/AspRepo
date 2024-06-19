using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace NotesSharingApp.Models.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public ICollection<Note> Notes { get; set; } = new List<Note>();
        public ICollection<File> Files { get; set; } = new List<File>();
    }
}
