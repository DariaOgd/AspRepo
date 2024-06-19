using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NotesSharingApp.Data;
using NotesSharingApp.Models.Entities;
using System;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using NotesSharingApp.Models;

namespace NotesSharingApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class FilesController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;

        public FilesController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpPost]
        public async Task<IActionResult> UploadFile([FromForm] AddFileDto addFileDto)
        {
            try
            {
                if (addFileDto.File == null || addFileDto.File.Length == 0)
                {
                    return BadRequest("No file uploaded.");
                }

                var allowedExtensions = new[] { ".pdf", ".doc", ".docx" };
                var fileExtension = Path.GetExtension(addFileDto.File.FileName).ToLowerInvariant();

                if (!allowedExtensions.Contains(fileExtension))
                {
                    return BadRequest("Invalid file type. Only PDF and Word documents are allowed.");
                }

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                var fileEntity = new NotesSharingApp.Models.Entities.File // Fully qualified name
                {
                    Id = Guid.NewGuid(),
                    FileName = addFileDto.File.FileName,
                    ContentType = addFileDto.File.ContentType,
                    Data = await ConvertFileToByteArrayAsync(addFileDto.File),
                    UserId = userId,
                    University = addFileDto.University, // Dodaj to pole
                    Title = addFileDto.Title, // Dodaj to pole
                    CategoryId = addFileDto.CategoryId // Dodaj to pole
                };

                _dbContext.Files.Add(fileEntity);
                await _dbContext.SaveChangesAsync();

                return Ok(fileEntity);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("all")]
        public async Task<IActionResult> GetAllFiles()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var files = await _dbContext.Files.Where(f => f.UserId == userId).ToListAsync();
                return Ok(files);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetFileById(Guid id)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var file = await _dbContext.Files.FirstOrDefaultAsync(f => f.Id == id && f.UserId == userId);

                if (file == null)
                {
                    return NotFound();
                }

                return Ok(file);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("download/{id:guid}")]
        public async Task<IActionResult> DownloadFile(Guid id)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var file = await _dbContext.Files.FirstOrDefaultAsync(f => f.Id == id && f.UserId == userId);

                if (file == null)
                {
                    return NotFound("File not found.");
                }

                return File(file.Data, file.ContentType, file.FileName);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetUserFiles(string userId)
        {
            try
            {
                var files = await _dbContext.Files.Where(f => f.UserId == userId).ToListAsync();
                return Ok(files);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("random")]
        public async Task<IActionResult> GetRandomFiles()
        {
            try
            {
                var randomFiles = await _dbContext.Files
                    .OrderBy(r => Guid.NewGuid())
                    .Take(4)
                    .ToListAsync();

                return Ok(randomFiles);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchFiles(string search = "")
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var filesQuery = _dbContext.Files
                    .Where(f => f.UserId == userId && (string.IsNullOrEmpty(search) || f.Title.Contains(search)))
                    .AsQueryable();

                var files = await filesQuery.ToListAsync();
                return Ok(files);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteFile(Guid id)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var file = await _dbContext.Files.FirstOrDefaultAsync(f => f.Id == id && f.UserId == userId);

                if (file == null)
                {
                    return NotFound("File not found.");
                }

                _dbContext.Files.Remove(file);
                await _dbContext.SaveChangesAsync();

                return Ok();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        private async Task<byte[]> ConvertFileToByteArrayAsync(IFormFile file)
        {
            using (var memoryStream = new MemoryStream())
            {
                await file.CopyToAsync(memoryStream);
                return memoryStream.ToArray();
            }
        }
    }
}
