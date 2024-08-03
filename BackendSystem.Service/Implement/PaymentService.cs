using BackendSystem.Respository.Implement;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Text;
using System.Threading.Tasks;

namespace BackendSystem.Service.Implement
{
    public class PaymentService
    {
        private IConfiguration config;
        private OrderRespository orderRespository;
        public PaymentService(IConfiguration _config,OrderRespository _orderRespository)
        {
            config = _config;
            orderRespository= _orderRespository;
        }

        public string PaymentCheck()
        {
            //訂單處理
            var order = orderRespository.GetOrder(1);

            //加密後的字串
            string? tradeInfo = "";
            string? tradeSha = "";

            string? merchan = config.GetSection("MerchantID").ToString();
            string? hashKey = config.GetSection("HashKey").ToString();
            string? hasdIV = config.GetSection("HashIV").ToString();
            string? version = "2.0";
            string? respondType = "String";
            string? merchantOrderNo = DateTime.Now.ToString() + "_"+ order.Id;
            string? itemDesc = "test";//商品資訊
            string? tradeLimit = "600"; // 交易限制秒數
            string timeStamp = ((int)(DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0)).TotalSeconds).ToString();
            //交易結果的通知網址
            string? notifyURL = @"https://NotifyURL";
            IDictionary<string,string?> Params = new Dictionary<string,string?>();
            Params.Add("MerchantID", merchan);
            Params.Add("Version", version);
            Params.Add("RespondType", respondType);
            Params.Add("ItemDesc", itemDesc);
            Params.Add("TradeLimit", tradeLimit);
            Params.Add("NotifyURL", notifyURL);
            Params.Add("TimeStamp", timeStamp);
            string tradeQuery = string.Join("&", Params.Select(res=>$"{res.Key}={res.Value}"));



            return "";
        }

        public string EncryptAES()
        {
            string str = @"";

            return str;
        }

        public string EncryptSHA()
        {
            string str = @"";

            return str;
        }

    }
}
