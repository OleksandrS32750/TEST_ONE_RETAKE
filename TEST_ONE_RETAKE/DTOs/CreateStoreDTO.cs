namespace TEST_ONE_RETAKE.DTOs
{
    public class CreateStoreDTO
    {
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;

        public ICollection<CreateBookDTO> BookDTOs { get; set; } = new List<CreateBookDTO>();
    }
}
