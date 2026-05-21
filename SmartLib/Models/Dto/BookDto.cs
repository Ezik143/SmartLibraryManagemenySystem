namespace SmartLib.Models.Dto
{
    public class BookDto
    {
        public int BookId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string Isbn { get; set; } = string.Empty;
        public string? Category { get; set; }
        public int? PublishedYear { get; set; }
        public int AvailableCopies { get; set; }
    }
}
