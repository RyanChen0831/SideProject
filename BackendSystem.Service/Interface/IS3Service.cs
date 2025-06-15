using Microsoft.AspNetCore.Http;

namespace BackendSystem.Service.Interface
{
    public interface IS3Service
    {
        public Task<List<string>> InsertProductImage(int productId ,List<IFormFile> file);

        public Task DeleteProductImage(int productId, List<string> images);

    }
}
