namespace Library.Models;

public class Author
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public ICollection<Book>? Books { get; set; }

    public override string ToString()
    {
        return $"{FirstName} {LastName}";
    }
}