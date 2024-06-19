using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NotesSharingApp.Data;
using NotesSharingApp.Models;
using NotesSharingApp.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NotesSharingApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CategoriesController : ControllerBase
    {
        private readonly ApplicationDbContext _dbContext;

        public CategoriesController(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoryDto>>> GetCategories()
        {
            var categories = await _dbContext.Categories
                .Include(c => c.Notes)
                .ThenInclude(n => n.User)
                .ToListAsync();

            var categoryDtos = categories.Select(c => new CategoryDto
            {
                Id = c.Id,
                Name = c.Name,
                Notes = c.Notes.Select(n => new NoteDto
                {
                    Id = n.Id,
                    Title = n.Title,
                    NoteBody = n.NoteBody,
                    CreatedAt = n.CreatedAt,
                    IsPublic = n.IsPublic,
                    University = n.University
                }).ToList()
            }).ToList();

            return Ok(categoryDtos);
        }

        [HttpPost]
        public async Task<ActionResult<CategoryDto>> AddCategory(CategoryDto categoryDto)
        {
            var category = new Category
            {
                Id = categoryDto.Id,
                Name = categoryDto.Name,
            };

            _dbContext.Categories.Add(category);
            await _dbContext.SaveChangesAsync();

            categoryDto.Id = category.Id;

            return CreatedAtAction(nameof(GetCategories), new { id = categoryDto.Id }, categoryDto);
        }
    }
}
