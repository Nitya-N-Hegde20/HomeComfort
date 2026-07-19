namespace HomeComfort.API.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string Image { get; set; }
        public decimal Rating { get; set; }
        public string? ReviewSummary { get; set; }
        public string? AmazonLink { get; set; }
        public string? FlipkartLink { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Foreign key
        public int CategoryId { get; set; }

        // Navigation property - product belongs to one category
        public Category? Category { get; set; }
    }
}
