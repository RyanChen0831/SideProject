using BackendSystem.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;

namespace BackendSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UploadPictureAPIController : ControllerBase
    {
        private readonly string _folder;
        private readonly long _fileSizeLimit;

        public UploadPictureAPIController(IConfiguration config)
        {
            _fileSizeLimit = config.GetValue<long>("FileSizeLimit");
            var date = DateTime.Now.ToString("yyyyMMdd");
            _folder = $"Images{Path.DirectorySeparatorChar}{date}";
            if (!Directory.Exists(_folder))
            {
                Directory.CreateDirectory(_folder);
            }
        }
        [HttpPost]
        public async Task<IActionResult> UploadData([FromForm]ProductImage files)
        {
            
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
