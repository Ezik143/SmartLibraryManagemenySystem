namespace SmartLib.Models.Dto.Create
{
    public class CreateBookDto
    {
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string Isbn { get; set; } = string.Empty;
        public string? Category { get; set; }
        public int? PublishedYear { get; set; }
        public int AvailableCopies { get; set; }
    }
}
