using Amazon;
using Amazon.S3;
using Amazon.S3.Transfer;
using BackendSystem.Models;
using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace BackendSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UploadPictureAPIController : ControllerBase
    {
        private readonly string _folder;
        private readonly long _fileSizeLimit;
        private readonly IDbConnection _dbConnection;

        public UploadPictureAPIController(IConfiguration config,IDbConnection dbConnection,IAmazonS3 amazonS3)
        {
            _fileSizeLimit = config.GetValue<long>("FileSizeLimit");
            var date = DateTime.Now.ToString("yyyyMMdd");
            _folder = $"Images{Path.DirectorySeparatorChar}{date}";
            if (!Directory.Exists(_folder))
            {
                Directory.CreateDirectory(_folder);
            }
            _dbConnection = dbConnection;
        }
        [HttpPost]
        [EnableCors("MyAllowSpecificOrigins")]
        [Authorize(Roles= "Admin")]
        public async Task<IActionResult> UploadData([FromForm]UploadData files)
        {

            var ImageMappingProductId = files.ProductId;
            var ImageName = files.Name;
            var ImgDescription =files.ImgDescription;

            if (files.Path.Count > 0)
            {
                foreach (var file in files.Path)
                {                   
                    if (CheckFileExtValid(file))
                    {
                        var fileName = file.FileName;
                        var ext = Path.GetExtension(fileName);
                        //避免重複檔案名稱產生使用GetRandomFileName()，產生亂數檔名。
                        var fileNewName = Path.GetRandomFileName();
                        var filePath = Path.Combine(_folder, fileNewName+ext);
                        using (var stream = System.IO.File.Create(filePath))
                        {
                            await file.CopyToAsync(stream);
                        }
                        var param = new ProductImage
                        {
                            ProductId = ImageMappingProductId,
                            Name = ImageName,
                            Path = filePath,
                            ImgDescription = ImgDescription
                        };

                        string str = @"INSERT INTO ProductImage (ProductId, Name, Path, ImgDescription) VALUES (@ProductId, @Name, @Path, @ImgDescription);";

                        // 使用 Dapper 執行 SQL 指令
                        _dbConnection.Execute(str, param);

                    }
                    else {
                        return BadRequest();
                    }
                }
            }
            return Ok();
        }

        //檢查檔案，副檔名限制、檔案大小的限制。
        private bool CheckFileExtValid(IFormFile file)
        {
            //取得檔案的大小。
            var size = file.Length;
            //允許的檔案格式，寫入一個陣列中。
            string[] vaildext = { ".jpg", ".png" };
            string ext = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (vaildext.Contains(ext) || !string.IsNullOrEmpty(ext)) 
            {
                if (size < _fileSizeLimit)
                {
                    return true;
                }
            }            
            return false;
        }

    }
}
