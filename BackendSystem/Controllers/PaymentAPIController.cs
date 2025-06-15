using BackendSystem.Dtos;
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
        [Authorize]
        public async Task<ActionResult<PaymentViewModel>> PaymentCheck([FromBody]TradingInfo tradingInfo)
        {
            var isSuccess = true;
            string productStr = "";
            int countAmount = 0;
            int memberId;
            try
            {
                // 從 HttpContext.User.Claims 中尋找使用者ID的 Claim
                var userIdClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);

                if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int _memberId))
                {
                    memberId = _memberId;
                }
                else
                {
                    return Unauthorized("找不到使用者");
                }

                var cartItem = await _shoppingCartService.GetCartItemAsync(memberId);
                if (cartItem == null || !(await CheckStock(cartItem)))  // 檢查庫存
                {
                    return BadRequest(new { Status = "Failed", Message = "庫存不足" });
                }

                await RemoveStock(cartItem);  // 扣庫存

                productStr = await CheckProduct(cartItem);
                countAmount = CountingAmount(cartItem);

                // 建立訂單信息
                string merchantID = _config["MerchantID"];
                string hashKey = _config["HashKey"]; //加密
                string hashIV = _config["HashIV"]; //加密
                string version = "2"; //金流版本
                string respondType = "String"; //回傳格式
                string itemDesc = productStr; // 商品信息
                string Amount = countAmount.ToString(); // 商品信息總金額
                string tradeLimit = "600"; // 交易限制秒數
                string timeStamp = DateTimeOffset.Now.ToUnixTimeSeconds().ToString();
                string customerEMail = tradingInfo.Email;
                string merchantOrderNo = DateTimeOffset.Now.ToString("yyyyMMddHHmmss") + "_" + timeStamp;//OrderId
                string notifyURL = @"https://eaf5-2001-b011-2017-588b-cdb6-f1d1-836b-a35c.ngrok-free.app/api/PaymentDone";
                string returnURL = $"https://localhost:5173/paymentresult?{merchantOrderNo}";                

                IDictionary<string, string> Params = new Dictionary<string, string>
                {
                    { "MerchantID", merchantID },
                    { "RespondType", respondType },
                    { "TimeStamp", timeStamp },
                    { "Version", version },
                    { "MerchantOrderNo", merchantOrderNo},
                    { "Amt", Amount},
                    { "ItemDesc", itemDesc },
                    { "TradeLimit", tradeLimit },
                    { "NotifyURL", notifyURL },
                    { "ReturnURL", returnURL },
                    { "Email", customerEMail}
                };

                string tradeQuery = string.Join("&", Params.Select(res => $"{res.Key}={res.Value}"));

                // AES 加密
                string tradeInfo = CryptoHelper.EncryptAES(tradeQuery, hashKey, hashIV);
                // SHA256 加密
                string tradeSha = CryptoHelper.EncryptSHA256($"HashKey={hashKey}&{tradeInfo}&HashIV={hashIV}");
                if (isSuccess) {
                    var order = new OrderInfo {
                        OrderId= merchantOrderNo,
                        MemberId = memberId,
                        TotalAmount = Convert.ToInt32(Amount),
                        ShippingAddress = tradingInfo.ShippingAddress,
                        ShippingStatus = "Processing",
                        Payment = tradingInfo.Payment,
                        PaymentStatus="Completed",
                    };
                    var createOrder = await CreateOrder(memberId, order);
                    var createOrderDetail = await CreateOrderDetail(merchantOrderNo, cartItem!);
                    await _shoppingCartService.ClearCartAsync(memberId);//清空購物車
                }
                return Ok(new PaymentViewModel
                {
                    Status = isSuccess ? "Success" : "Failed",
                    MerchantID = merchantID,
                    TradeInfo = isSuccess ? tradeInfo : null,
                    TradeSha = isSuccess ? tradeSha : null,
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
        public IActionResult PayDone([FromBody] PaymentViewModel payment)
        {
            try
            {
                string hashKey = _config["HashKey"];
                string hashIV = _config["HashIV"];

                // 解密 TradeInfo
                var decryptedString = CryptoHelper.DecryptAES(payment.TradeInfo!, hashKey, hashIV);
                NameValueCollection requestParams = HttpUtility.ParseQueryString(decryptedString!);

                // 取出訂單號
                string orderId = requestParams["MerchantOrderNo"]!;
                string status = requestParams["Status"]!; // 交易狀態

                if (status == "SUCCESS")
                {
                    var order = new OrderStatusInfo()
                    {
                        OrderId = orderId,
                        StatusId = 2, // 設為已付款
                    };

                    _orderService.UpdateOrderStatus(order);
                    return Ok("SUCCESS");
                }

                return BadRequest("交易失敗");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Server Error: {ex.Message}");
            }
        }



        /// <summary>
        /// 付款完成後建立訂單
        /// </summary>
        /// <param name="usesId"></param>
        /// <returns></returns>
        private async Task<bool> CreateOrder(int memberId,OrderInfo order)
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
                var product = await _productService.GetProduct(item.ProductId);
                if (product != null)
                {
                    orderDetail.OrderId = orderId;
                    orderDetail.ProductId = product.ProductId;
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
                    var product = await _productService.GetProduct(item.ProductId);
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
                var product = _productService.GetProduct(item!.ProductId).Result;
                return product!.Price * item.Quantity;
            });

            return totalAmount;
        }
        private async Task<bool> CheckStock(List<ShoppingCartViewModel?> cartItem)
        {
            var result = true;
            foreach (var item in cartItem)
            {
                result = await _productService.CheckProductStock(item.Quantity,item.ProductId);
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
                result = await _productService.RemoveProductStock(item.Quantity, item.ProductId);
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
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Payment { get; set; }
        public string ShippingAddress { get; set; }
        public string PaymentMethod { get; set; }

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
