namespace TEST_ONE_RETAKE.DTOs
{
    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Synopsis { get; set; } 
        public decimal BasePrice { get; set; }

        public Genre Genre { get; set; } = null!;

        public Author Author { get; set; } = null!;
    }
}
