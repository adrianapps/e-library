namespace Library.Models
{
    public class CartItem
    {
        public int BookId { get; set; }
        public string Title { get; set; }
        public string ISBN { get; set; }
        public string? Cover { get; set; } 
        public string AuthorName { get; set; } 
    }
}
