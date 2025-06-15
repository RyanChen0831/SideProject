using AutoMapper;
using BackendSystem.Dtos;
using BackendSystem.Service.Dtos;
using BackendSystem.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BackendSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [EnableCors("MyAllowSpecificOrigins")]
    public class ProductAPIController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IMapper _mapper;
        private readonly ILogger<ProductAPIController> _logger;
        public ProductAPIController(IProductService productService,IMapper mapper, ILogger<ProductAPIController> logger)
        {
            _productService = productService;
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IEnumerable<ProductViewModel?>> GetProductList()
        {
            var products = await _productService.GetProductList();
            return products;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProductViewModel?>> GetProduct(int id)
        {
            var product = await _productService.GetProduct(id);
            return Ok(product);
        }

        [HttpPost("Create")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<int>> Create(NewProductParameter info)
        {
            var param = _mapper.Map<ProductInfo>(info);
            try
            {
                int productId = await _productService.Create(param);
                return Ok(new { productId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create product");
                return StatusCode(500, "Internal Server Error");
            }
        }

        [HttpPost("Update")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateProduct(ProductInfo param)
        {
            bool result = await _productService.UpdateProduct(param);

            if (result)
            {
                return Ok();
            }
            else
            {
                return NotFound();
            }

        }

        [HttpDelete("{productid}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int productid)
        {
            bool result = await _productService.Delete(productid);

            if (result)
            {
                return Ok();
            }
            else
            {
                return NotFound();
            }

        }

        [HttpPatch("UpdateProductStatus")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateProductStatus([FromBody] UpdateProductStatusParameter parameter)
        {
            bool result = await _productService.UpdateProdcutStatus(parameter.ProductId, parameter.Status);

            if (result)
            {
                return Ok();
            }
            else
            {
                return BadRequest();
            }

        }

        [HttpGet("GetProductCategory")]
        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> GetProductCategory()
        {
            var res = await _productService.GetProductCategory();

            return Ok(res);
        }

    }
}
