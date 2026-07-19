using HomeComfort.API.Data;
using HomeComfort.API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace HomeComfort.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMemoryCache _cache;

        public CategoriesController(AppDbContext appDbContext, IMemoryCache cache)
        {
            _context = appDbContext;
           _cache = cache;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Category>>> GetCategories()
        {
            if (!_cache.TryGetValue("categories", out List<Category>? categories))
            {
                categories =await _context.Categories.ToListAsync();
                _cache.Set("categories",categories, TimeSpan.FromSeconds(10));
                    
            }
            return Ok(categories);
        }


        [HttpPost]
        public async Task<ActionResult<Category>> CreateCategory(Category category)
        {
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            _cache.Remove("categories");

            return CreatedAtAction(nameof(GetCategories), new { id = category.Id }, category);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);

            if (category == null)
            {
                return NotFound();
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            _cache.Remove("categories");

            return NoContent();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Category>> GetCategory(int id)
        {
            string cacheKey = $"category_{id}";

            if (!_cache.TryGetValue(cacheKey, out Category? category))
            {
                category = await _context.Categories.FindAsync(id);

                if (category == null)
                {
                    return NotFound();
                }

                _cache.Set(cacheKey, category, TimeSpan.FromMinutes(10));
            }

            return Ok(category);
        }

    }
}
