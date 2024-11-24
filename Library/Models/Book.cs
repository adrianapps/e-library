using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Library.Models;

public class Book
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string ISBN { get; set; }
    public string Description { get; set; }
    [DataType(DataType.Date)] 
    public DateTime ReleaseDate { get; set; }

    public int CategoryId { get; set; }
    [ForeignKey("CategoryId")]
    public Category? Category { get; set; }
    
    public int AuthorId { get; set; }
    [ForeignKey("AuthorId")]
    public Author? Author { get; set; }
    
    public ICollection<BookFile>? Files { get; set; } 
    public int Stock { get; set; }
    public ICollection<Borrow>? Borrows { get; set; }
}