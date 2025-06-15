using BackendSystem.Service.Interface;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace BackendSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("MyAllowSpecificOrigins")]
    public class AmazonS3Controller : ControllerBase
    {
        private readonly IS3Service _s3Service;

        public AmazonS3Controller(IS3Service s3Service)
        {
            _s3Service = s3Service;
        }

        [HttpPost("products/{productId}/images")]
        public async Task<ActionResult> UploadPicture(int productId ,List<IFormFile> file) 
        {
            if (file == null || file.Count == 0)
            {
                return BadRequest("未選擇檔案");
            }
            try
            {
                var res = await _s3Service.InsertProductImage(productId, file);
                return Ok(new { Message = "Success", Files = res });
            }
            catch (Exception e)
            {
                return StatusCode(500, new { Message = e.Message });
            }
        }

        [HttpPost("products/{productId}/images/delete")]
        public async Task<IActionResult> DeletePicture(int productId, List<string> list) 
        {
            try
            {
                await _s3Service.DeleteProductImage(productId, list);
                return Ok();
            }
            catch (Exception)
            {
                return BadRequest();
                throw;
            }
            
        }

    }
}
