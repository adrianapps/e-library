using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Library.Models;

public class BookFile
{
    public int Id { get; set; }
    public string FileName { get; set; }
    public string Description { get; set; }
    public string FilePath { get; set; }
    
    public int BookId { get; set; }
    [ForeignKey("BookId")]
    public Book Book { get; set; }
}