using TEST_ONE_RETAKE.DTOs;

namespace TEST_ONE_RETAKE.Repositories
{
    public interface IStoreService
    {
        Task<STORE_DTO> GetAllStoresAsync(string? name);

        Task<string?> CreateStoreAsync(CreateStoreDTO createStoreDTO);
    }
}
