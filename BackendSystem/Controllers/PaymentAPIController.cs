using BackendSystem.DTO;
using BackendSystem.Respository.Implement;
using BackendSystem.Service.Dtos;
using BackendSystem.Service.Implement;
using BackendSystem.Service.Interface;
using BackendSystem.Service.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using StackExchange.Redis;
using System.Collections.Specialized;
using System.Data;
using System.Security.Claims;
using System.Security.Policy;
using System.Text;
using System.Web;

namespace BackendSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("MyAllowSpecificOrigins")]
    public class PaymentAPIController : ControllerBase
    {
        private IConfiguration _config;
        private IOrderService _orderService;
        private IShoppingCartService _shoppingCartService;
        private IProductService _productService;
        public PaymentAPIController(IConfiguration config, IOrderService orderService, IShoppingCartService shoppingCartService, IProductService productService)
        {
            _config = config;
            _orderService = orderService;
            _shoppingCartService = shoppingCartService;
            _productService = productService;
        }

        /// <summary>
        /// 負責藍新金流API 的資訊加密及參數設定
        /// </summary>
        /// <param name=""></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<PaymentViewModel>> PaymentCheck(TradingInfo tradingInfo)
        {
            var isSuccess = true;
            string productStr = "";
            int countAmount = 0;
            int userId;
            try
            {
                // 從 HttpContext.User.Claims 中尋找使用者ID的 Claim
                var userIdClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);

                if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int _userId))
                {
                    userId = _userId;
                }
                else
                {
                    return Unauthorized("找不到使用者");
                }

                var cartItem = await _shoppingCartService.GetCartItemAsync(userId);
                //將購物車中的所有商品進行庫存檢查
                var isAvailable = await CheckStock(cartItem);
                if (isAvailable)
                {
                    await RemoveStock(cartItem);
                }
                else
                {
                    isSuccess = false;
                }

                if (cartItem == null )
                {
                    isSuccess = false;
                }
                else
                {
                    productStr = await CheckProduct(cartItem);
                    countAmount = CountingAmount(cartItem);
                }

                // 建立訂單信息
                string merchantID = _config["MerchantID"];
                string hashKey = _config["HashKey"]; //加密
                string hashIV = _config["HashIV"]; //加密
                string version = "2.0"; //金流版本
                string respondType = "Json"; //回傳格式
                string itemDesc = productStr; // 商品信息
                int Amount = countAmount; // 商品信息總金額
                string tradeLimit = "600"; // 交易限制秒數
                string timeStamp = DateTimeOffset.Now.ToUnixTimeSeconds().ToString();
                string notifyURL = @"https://NotifyURL";
                string returnURL = @"https://NotifyURL";
                string customerEMail = tradingInfo.Email;
                string merchantOrderNo = DateTimeOffset.Now.ToString("yyyyMMddHHmmss") + "_" + timeStamp;//OrderId

                IDictionary<string, string> Params = new Dictionary<string, string>
                {
                    { "MerchantID", merchantID },
                    { "Version", version },
                    { "RespondType", respondType },
                    { "ItemDesc", itemDesc },
                    { "TradeLimit", tradeLimit },
                    { "NotifyURL", notifyURL },
                    { "TimeStamp", timeStamp },
                    { "ReturnURL", returnURL },
                    { "Email", customerEMail},
                    { "MerchantOrderNo", merchantOrderNo}
                };

                string tradeQuery = string.Join("&", Params.Select(res => $"{res.Key}={res.Value}"));

                // AES 加密
                string tradeInfo = CryptoHelper.EncryptAES(tradeQuery, hashKey, hashIV);
                // SHA256 加密
                string tradeSha = CryptoHelper.EncryptSHA256($"HashKey={hashKey}&{tradeInfo}&HashIV={hashIV}");
                if (isSuccess) {
                    var order = new OrderInfo {
                        OrderId= merchantOrderNo,
                        UserID = userId,
                        TotalAmount = Amount,
                        ShippingAddress = tradingInfo.shippingAddress,
                        ShippingStatus = "Processing",
                        Payment = tradingInfo.Payment,
                        PaymentStatus="Depending",
                    };
                    var createOrder = await CreateOrder(userId, order);
                    var createOrderDetail = await CreateOrderDetail(merchantOrderNo, cartItem!);

                    ////清空購物車
                    //await _shoppingCartService.ClearCartAsync(userId);

                }
                return Ok(new PaymentViewModel
                {
                    Status = isSuccess ? "Success" : "Failed",
                    MerchantID = merchantID,
                    TradeInfo = tradeInfo,
                    TradeSha = tradeSha,
                    Version = version
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Server Error: {ex.Message}");
            }
        }

        /// <summary>
        /// 付款完成進行解密交易資訊及更新訂單狀態
        /// </summary>
        [HttpPost("PayDone")]
        public void PayDone([FromBody]PaymentViewModel payment)
        {
            string hashKey = _config["HashKey"]; 
            string hashIV = _config["HashIV"];
            var decrytostring = CryptoHelper.DecryptAES(payment.TradeInfo!, hashKey, hashIV);
            //解密字串
            NameValueCollection Request = HttpUtility.ParseQueryString(decrytostring!);
            //取出訂單號
            string orderID = Request["MerchantOrderNo"]!;
            //更改訂單狀態
            var order = new OrderStatusInfo() 
            { 
                OrderID = orderID,
                StatusID = 2,
            };
            _orderService.UpdateOrderStatus(order);
            //待完成
            //寄出訂單的連結到使用者的Mail


        }


        /// <summary>
        /// 付款完成後建立訂單
        /// </summary>
        /// <param name="usesId"></param>
        /// <returns></returns>
        private async Task<bool> CreateOrder(int userId,OrderInfo order)
        {
            return await _orderService.CreateOrder(order);
        }

        /// <summary>
        /// 付款完成後建立訂單明細
        /// </summary>
        /// <param name="usesId"></param>
        /// <returns></returns>
        private async Task<bool> CreateOrderDetail(string orderId,List<ShoppingCartViewModel> cartItem)
        {
            foreach (var item in cartItem)
            {
                var orderDetail = new OrderDetailInfo();
                var product = await _productService.GetProduct(item.ProductID);
                if (product != null)
                {
                    orderDetail.OrderId = orderId;
                    orderDetail.ProductID = product.ProductID;
                    orderDetail.UnitPrice = product.Price;
                    orderDetail.Quantity= item.Quantity;
                    orderDetail.SubTotal = product.Price * item.Quantity;
                }
                await _orderService.CreateOrderDetail(orderDetail);
            }
            return true;
        }

        /// <summary>
        /// 組成商品信息
        /// </summary>
        /// <param name="cartItem"></param>
        /// <returns></returns>
        private async Task<string> CheckProduct(List<ShoppingCartViewModel?> cartItem)
        {
            var itemDesc = new StringBuilder();
            foreach (var item in cartItem)
            {
                if (item != null)
                {
                    var product = await _productService.GetProduct(item.ProductID);
                    if (product != null)
                    {
                        // 確保商品名稱長度不超過50字元
                        var productName = product.ProductName!.Length > 50 ? product.ProductName.Substring(0, 50) : product.ProductName;

                        // 確保商品名稱中不包含特殊符號，並轉換為 Utf-8 格式
                        productName = RemoveSpecialCharacters(productName);

                        itemDesc.Append(productName);
                    }
                }
            }
            return itemDesc.ToString();
        }

        /// <summary>
        /// 移除特殊字元
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private string RemoveSpecialCharacters(string input)
        {
            StringBuilder sb = new StringBuilder();
            foreach (char c in input)
            {
                // 如果字元是字母或數字，則添加到 StringBuilder 中
                if (char.IsLetterOrDigit(c))
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }

        /// <summary>
        /// 計算本次購物車內的總金額
        /// </summary>
        /// <param name="cartItem"></param>
        /// <returns></returns>
        private int CountingAmount(List<ShoppingCartViewModel?> cartItem)
        {
            // 使用 LINQ 查詢每個商品的價格，然後乘以數量，最後加總
            var totalAmount = cartItem.Sum(item =>
            {
                var product = _productService.GetProduct(item!.ProductID).Result;
                return product!.Price * item.Quantity;
            });

            return totalAmount;
        }
        private async Task<bool> CheckStock(List<ShoppingCartViewModel?> cartItem)
        {
            var result = true;
            foreach (var item in cartItem)
            {
                result = await _productService.CheckProductStock(item.Quantity,item.ProductID);
                if (!result)
                {
                    return false;
                }
            }
            return result;
        }

        private async Task<bool> RemoveStock(List<ShoppingCartViewModel?> cartItem)
        {
            var result = true;
            foreach (var item in cartItem)
            {
                result = await _productService.RemoveProductStock(item.Quantity, item.ProductID);
                if (!result)
                {
                    return false;
                }
            }
            return result;
        }


    }
    public class TradingInfo
    {
        public string Email { get; set; }
        public string Payment { get; set; }
        public string shippingAddress { get; set; }

    }
    public class PaymentResult
    {
        public string Status { get; set; }
        public string Message { get; set; }
        public Result Result { get; set; }
    }

    public class Result
    {
        public string MerchantID { get; set; }
        public string Amt { get; set; }
        public string TradeNo { get; set; }
        public string MerchantOrderNo { get; set; }
        public string RespondType { get; set; }
        public string IP { get; set; }
        public string EscrowBank { get; set; }
        public string PaymentType { get; set; }
        public string RespondCode { get; set; }
        public string Auth { get; set; }
        public string Card6No { get; set; }
        public string Card4No { get; set; }
        public string Exp { get; set; }
        public string TokenUseStatus { get; set; }
        public string InstFirst { get; set; }
        public string InstEach { get; set; }
        public string Inst { get; set; }
        public string ECI { get; set; }
        public string PayTime { get; set; }
        public string PaymentMethod { get; set; }
    }
}
