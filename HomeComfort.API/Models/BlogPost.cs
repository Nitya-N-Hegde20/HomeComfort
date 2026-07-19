namespace HomeComfort.API.Models
{
    public class BlogPost
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string Author { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;
        public string Image { get; set; }
        public string Category { get; set; }
        public int ReadTime { get; set; }
    }
}
