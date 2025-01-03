namespace Library.Models;

public class Message
{
    public int Id { get; set; }
    public String Title { get; set; }
    public String Description { get; set; }
    public DateTime Timestamp { get; set; }

    public override string ToString()
    {
        return Title;
    }
}