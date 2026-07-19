using HomeComfort.API.Data;
using HomeComfort.API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace HomeComfort.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMemoryCache _cache;

        public ProductsController(AppDbContext context, IMemoryCache cache)
        {
            _context = context;
            _cache = cache;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Product>>> GetProducts()
        {
            if (!_cache.TryGetValue("products", out List<Product>? products))
            {
                products = await _context.Products.Include(p => p.Category).ToListAsync();
                _cache.Set("products", products, TimeSpan.FromMinutes(10));
            }
            return Ok(products);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Product>> GetProduct(int id)
        {
            string cacheKey = $"product_{id}";

            if (!_cache.TryGetValue(cacheKey, out Product? product))
            {
                product = await _context.Products.FindAsync(id);

                if (product == null)
                {
                    return NotFound();
                }

                _cache.Set(cacheKey, product, TimeSpan.FromMinutes(10));
            }

            return Ok(product);
        }


        [HttpPost]
        public async Task<ActionResult<Product>> CreateProduct(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            _cache.Remove("products"); // invalidate the list cache

            return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, Product product)
        {
            if (id != product.Id)
            {
                return BadRequest();
            }

            _context.Entry(product).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Products.AnyAsync(p => p.Id == id))
                {
                    return NotFound();
                }
                throw;
            }

            _cache.Remove("products");
            _cache.Remove($"product_{id}");

            return NoContent();
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                return NotFound();
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            _cache.Remove("products");
            _cache.Remove($"product_{id}");

            return NoContent();
        }
    }
}
