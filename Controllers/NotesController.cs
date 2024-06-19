using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NotesSharingApp.Data;
using NotesSharingApp.Models;
using NotesSharingApp.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace NotesSharingApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class NotesController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<NotesController> _logger;

        public NotesController(ApplicationDbContext dbContext, ILogger<NotesController> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Note>>> GetAllNotes(string search = "", bool? isPublic = null, string university = "", Guid? categoryId = null)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var notesQuery = _dbContext.Notes
                .Where(n => (n.UserId == userId || n.IsPublic) &&
                            (string.IsNullOrEmpty(search) || n.Title.Contains(search) || n.NoteBody.Contains(search)) &&
                            (isPublic == null || n.IsPublic == isPublic) &&
                            (string.IsNullOrEmpty(university) || n.University == university) &&
                            (categoryId == null || n.CategoryId == categoryId))
                .AsQueryable();

            var allNotes = await notesQuery.ToListAsync();

            return Ok(allNotes);
        }

        [HttpGet("{id:guid}")]
        public async Task<ActionResult<Note>> GetNoteById(Guid id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var note = await _dbContext.Notes
                .FirstOrDefaultAsync(n => n.Id == id && (n.UserId == userId || n.IsPublic));

            if (note == null)
            {
                return NotFound();
            }
            return Ok(note);
        }

        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<Note>>> GetUserNotes(string userId)
        {
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Sprawdź, czy użytkownik ma uprawnienia do przeglądania notatek innego użytkownika, jeśli to konieczne
            if (currentUserId != userId)
            {
                // Tutaj możesz dodać logikę, aby sprawdzić, czy użytkownik ma odpowiednie uprawnienia
                // Jeżeli nie ma, możesz zwrócić 403 Forbidden lub inny odpowiedni status
                return Forbid();
            }

            var userNotes = await _dbContext.Notes
                .Where(n => n.UserId == userId)
                .ToListAsync();

            return Ok(userNotes);
        }

        [HttpPost]
        public async Task<ActionResult<Note>> AddNote([FromBody] AddNoteDto addNoteDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var noteEntity = new Note()
            {
                Title = addNoteDto.Title,
                NoteBody = addNoteDto.NoteBody,
                CreatedAt = DateTime.UtcNow,
                UserId = userId,
                IsPublic = addNoteDto.IsPublic,
                University = addNoteDto.University,
                CategoryId = addNoteDto.CategoryId
            };

            _dbContext.Notes.Add(noteEntity);
            await _dbContext.SaveChangesAsync();

            return CreatedAtAction(nameof(GetNoteById), new { id = noteEntity.Id }, noteEntity);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateNote(Guid id, [FromBody] UpdateNoteDto updateNoteDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var note = await _dbContext.Notes.FirstOrDefaultAsync(n => n.Id == id && n.UserId == userId);

            if (note == null)
            {
                return NotFound();
            }

            note.Title = updateNoteDto.Title;
            note.NoteBody = updateNoteDto.NoteBody;
            note.IsPublic = updateNoteDto.IsPublic;
            note.University = updateNoteDto.University;
            note.CategoryId = updateNoteDto.CategoryId;
            await _dbContext.SaveChangesAsync();
            return Ok(note);
        }


        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteNote(Guid id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var note = await _dbContext.Notes.FirstOrDefaultAsync(n => n.Id == id && n.UserId == userId);

            if (note == null)
            {
                return NotFound();
            }

            _dbContext.Notes.Remove(note);
            await _dbContext.SaveChangesAsync();
            return Ok();
        }
    }
}
