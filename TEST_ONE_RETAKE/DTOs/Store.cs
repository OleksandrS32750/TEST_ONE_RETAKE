using System.Numerics;

namespace TEST_ONE_RETAKE.DTOs
{
    public class Store
    {
        public string Code = string.Empty;
        public string Name = string.Empty;

        public ICollection<Book> Books { get; set; } = new List<Book>();
    }
}
