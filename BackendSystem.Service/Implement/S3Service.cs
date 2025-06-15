using Amazon.S3;
using Amazon.S3.Transfer;
using Amazon;
using BackendSystem.Respository.Dtos;
using BackendSystem.Service.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Amazon.Runtime;

namespace BackendSystem.Service.Implement
{
    public class S3Service : IS3Service
    {
        private readonly IAmazonS3 _s3Client;
        private readonly string _bucketName;
        private readonly IProductRespository _productRespository;

        public S3Service(IAmazonS3 s3Client, IConfiguration configuration, IProductRespository productRespository)
        {

            _bucketName = configuration["AWS:S3:BucketName"];
            var region = configuration["AWS:S3:Region"];
            _s3Client = new AmazonS3Client(RegionEndpoint.GetBySystemName(region));
            _productRespository = productRespository;
        }

        public async Task<List<string>> InsertProductImage(int productId, List<IFormFile> files)
        {
            var transfer = new TransferUtility(_s3Client);
            var list = new List<string>();
            foreach (var file in files)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                string s3Key = $"product-image/{productId}/{fileName}";
                using Stream stream = file.OpenReadStream();
                await transfer.UploadAsync(stream, _bucketName, s3Key);

                string url = $"https://{_bucketName}.s3.amazonaws.com/{s3Key}";

                //將路徑存到資料庫
                await _productRespository.InsertProductImage(new ProductImage
                {
                    ProductId = productId,
                    Name = fileName,
                    Path = s3Key
                });
                list.Add(url);
            }
            return list;
        }

        public async Task DeleteProductImage(int productId, List<string> images)
        {
            try
            {
                await _productRespository.DeleteProductImage(productId, images);
                foreach (var image in images)
                {
                    string s3Key = $"product-image/{productId}/{image}";
                    await _s3Client.DeleteObjectAsync(_bucketName, s3Key);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
