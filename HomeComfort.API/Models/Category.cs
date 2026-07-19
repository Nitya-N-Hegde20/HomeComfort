namespace HomeComfort.API.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public DateTime CreatedAt { get; set; } 

        // Navigation property - one category has many products
       // public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
