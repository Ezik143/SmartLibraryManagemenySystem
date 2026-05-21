namespace SmartLib.Models.Entities;

using Microsoft.EntityFrameworkCore;


[Index(nameof(Isbn), IsUnique = true)]
public class Book
{
    public int BookId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public string Isbn { get; set; } = string.Empty;
    public string? Category { get; set; }
    public int? PublishedYear { get; set; }
    public int TotalCopies { get; set; } = 1;
    public int AvailableCopies { get; set; } = 1;
    public DateTime CreatedAt { get; set; }
}
