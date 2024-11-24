using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Library.Models;

public class Borrow
{
    public int Id { get; set; }
    public int BookId { get; set; }
    [ForeignKey("BookId")]
    public Book Book { get; set; }
    public string UserId { get; set; }
    [ForeignKey("UserId")]
    public User User { get; set; }
    public DateTime BorrowDate { get; set; }
    public DateTime? ReturnDate { get; set; }
    public BorrowStatus Status { get; set; }
}

public enum BorrowStatus
{
    Returned,
    Borrowed
}