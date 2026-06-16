using TEST_ONE_RETAKE.DTOs;
using TEST_ONE_RETAKE.Repositories;

namespace TEST_ONE_RETAKE.Services
{
    public class StoreService :IStoreService
    {
        private readonly IStoreRepository _storeRepository;

        public StoreService(IStoreRepository storeRepository)
        {
            _storeRepository = storeRepository;
        }

        public async Task<STORE_DTO?> GetAllStoresAsync(string? name)
        {
            return await _storeRepository.GetAllStoresAsync(name);
        }

        public async Task<string?> CreateStoreAsync(CreateStoreDTO createStoreDto)
        {
            return await _storeRepository.CreateStoreAsync(createStoreDto);
        }
    }
}
