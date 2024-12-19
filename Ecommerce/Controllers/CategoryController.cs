using Ecommerce.Models;
using Ecommerce.Models.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ecommerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CategoryController> _logger;

        public CategoryController(ApplicationDbContext context, ILogger<CategoryController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoryResponseDTO>>> GetCategories()
        {
            try
            {
                var categories = await _context.Categories
                    .Include(c => c.Products)  
                    .ToListAsync();

                if (!categories.Any())
                {
                    return NotFound("No categories found.");
                }

                var categoryResponses = categories.Select(CategoryResponseDTO.FromCategory);
                return Ok(categoryResponses);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching categories.");
                return StatusCode(500, "Internal server error.");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CategoryResponseDTO>> GetCategory(int id)
        {
            try
            {
                var category = await _context.Categories
                    .Include(c => c.Products)
                    .FirstOrDefaultAsync(c => c.Id == id);

                if (category == null)
                {
                    return NotFound($"Category with ID {id} not found.");
                }

                return Ok(CategoryResponseDTO.FromCategory(category));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error fetching category with ID {id}.");
                return StatusCode(500, "Internal server error.");
            }
        }

            [Authorize(Roles = "Admin")]
            [HttpPost]
            public async Task<ActionResult<CategoryResponseDTO>> CreateCategory(CategoryDTO categoryDto)
            {
                try
                {
                    if (!ModelState.IsValid)
                    {
                        return BadRequest(ModelState);  
                    }

                    if (categoryDto == null)
                    {
                        return BadRequest("Category data is required.");
                    }

                    var category = new Category
                    {
                        Name = categoryDto.Name
                    };

                    _context.Categories.Add(category);
                    await _context.SaveChangesAsync();

                    await _context.Entry(category)
                        .Collection(c => c.Products)
                        .LoadAsync();

                    var responseDto = CategoryResponseDTO.FromCategory(category);
                    return CreatedAtAction(
                        nameof(GetCategory),
                        new { id = category.Id },
                        responseDto
                    );
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error creating category.");
                    return StatusCode(500, "Internal server error.");
                }
            }


        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            try
            {
                var category = await _context.Categories.FindAsync(id);

                if (category == null)
                {
                    return NotFound($"Category with ID {id} not found.");
                }

                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting category with ID {id}.");
                return StatusCode(500, "Internal server error.");
            }
        }
    }
}
