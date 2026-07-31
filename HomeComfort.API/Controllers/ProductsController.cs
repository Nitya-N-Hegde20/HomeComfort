using HomeComfort.API.Data;
using HomeComfort.API.Models;
using HomeComfort.API.Services;
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
        private readonly NotificationService _notificationService;
        private readonly ServiceBusPublisher _serviceBus;

        public ProductsController(AppDbContext context, IMemoryCache cache, NotificationService notificationService, ServiceBusPublisher serviceBus)
        {
            _context = context;
            _cache = cache;
            _notificationService = notificationService;
            _serviceBus = serviceBus;
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
            await _serviceBus.PublishProductCreated(product.Id, product.Name);

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

        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<Product>>> SearchProducts([FromQuery] string term)
        {
            if (string.IsNullOrWhiteSpace(term))
            {
                return BadRequest("Search term is required.");
            }

            var results = await _context.Products
                .Include(p => p.Category)
                .Where(p => p.Name.Contains(term) ||
                            p.Description.Contains(term))
                .ToListAsync();

            if (results.Count == 0)
            {
                var normalizedTerm = term.Trim().ToLowerInvariant();
                var cacheKey = $"missing-product-{normalizedTerm}";

                if (!_cache.TryGetValue(cacheKey, out _))
                {
                    await _notificationService.SendMissingProductAlert(term);

                    _cache.Set(
                        cacheKey,
                        true,
                        TimeSpan.FromHours(24)); 

                    Console.WriteLine($"Notification sent for: {term}");
                }
                else
                {
                    Console.WriteLine($"Notification already sent recently for: {term}");
                }
            }

            return Ok(results);
        }

        [HttpPost("notify-missing")]
        public async Task<IActionResult> NotifyMissingProduct([FromBody] string searchTerm)
        {
            string cacheKey = $"notified_{searchTerm.ToLower().Trim()}";

            if (!_cache.TryGetValue(cacheKey, out _))
            {
                await _notificationService.SendMissingProductAlert(searchTerm);
                _cache.Set(cacheKey, true, TimeSpan.FromHours(24));
            }

            return Ok();
        }
    }
}
