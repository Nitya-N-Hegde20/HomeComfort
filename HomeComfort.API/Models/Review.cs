namespace HomeComfort.API.Models
{
    public class Review
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int Rating { get; set; }
        public string Comment { get; set; }
        public string Author { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;
    }
}
