using BackendSystem.Models;
using BackendSystem.Service.Dtos;
using BackendSystem.Service.Interface;
using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Hosting.Internal;
using System;
using System.Configuration;
using System.Data;
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
        private readonly IDbConnection _connection;
        public ProductAPIController(IProductService productService, IDbConnection connection)
        {
            _productService = productService;
            _connection = connection;
        }

        [HttpGet]
        public IEnumerable<ProductViewModel?> GetProductList()
        {
            var products = _productService.GetProductList();
            return products;
        }

       [HttpGet("{id}")]
        public async Task<ActionResult<ProductViewModel?>> GetProduct(int id )
        {
            var userIdClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if(userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
            {
                Console.WriteLine($"User ID: {userId}");
            }
            else
            {
                Console.WriteLine($"找不到使用者ID");
            }
            var product = await _productService.GetProduct(id);
            return Ok(product);
        }

        [HttpPost("Create")]
        [Authorize(Roles="Admin")]
        public async Task<bool> Create(ProductInfo param)
        {
           return await _productService.Create(param);
        }

        [HttpPost("Update")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<bool>> UpdateProduct(ProductInfo param)
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

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<bool>> Delete(int id)
        {
            bool result = await _productService.Delete(id);

            if (result)
            {
                return Ok();
            }
            else
            {
                return NotFound();
            }

        }
    }
}
