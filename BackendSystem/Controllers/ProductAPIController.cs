using BackendSystem.Models;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Hosting.Internal;
using System;
using System.Configuration;
using System.Data;

namespace BackendSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductAPIController : ControllerBase
    {
        private readonly IDbConnection _dbConnection;
        public ProductAPIController(IDbConnection dbConnection)
        {
            _dbConnection = dbConnection;
        }

        [HttpGet]
        public IEnumerable<Product> GetProductList()
        {
            string str = @"SELECT * FROM Product";
            var product = _dbConnection.Query<Product>(str);
            return product;
        }

        [HttpGet("id")]
        public IEnumerable<Product> GetProduct(int id )
        {
            string str = @"SELECT * FROM Product WHERE ProductId = @ProductId";
            //在使用DynamicParameters會協助隱含轉換型別，例如int、bool等較常見的型別。
            var param = new DynamicParameters();
            //也可以指定強型別
            param.Add("ProductId", id, System.Data.DbType.Int32);
            var product = _dbConnection.Query<Product>(str, param);
            return product;
        }

        [HttpPost]
        public string Create(Product param)
        {
            string str = @"INSERT INTO Product (ProductId, ProductName, Price, Description) VALUES (@ProductId, @ProductName, @Price, @Description);";
            //直接指定輸入的物件格式
            _dbConnection.Execute(str, param);
            return "Success!!";
        }

        [HttpDelete("id")]
        public string Delete(int id)
        {
            string str = @"DELETE FROM Product WHERE ProductId=@ProductId";
            var param = new DynamicParameters();
            param.Add("ProductId", id, System.Data.DbType.Int32);
            _dbConnection.Execute(str, param);
            return "Delete Success!!";
        }
    }
}
