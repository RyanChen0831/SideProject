using BackendSystem.Service.Interface;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Text;

namespace BackendSystem.Service.Security
{
    public class TokenManager:ITokenService
    {
        private readonly IConfiguration _configuration;
        public TokenManager(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public (User?, bool) DecodeToken(Token token)
        {
            try
            {
                // 提取 IV、加密的 payload 和簽名
                var parts = token.AccessToken.Split('.');
                if (parts.Length != 3)
                {
                    return (null, false);
                }

                var iv = parts[0];
                var encryptedPayload = parts[1];
                var signature = parts[2];

                var secretKey = _configuration["JWTSetting:SignKey"];

                // 驗證簽名
                var computedSignature = CryptoHelper.GenerateHmacSha256($"{iv}.{encryptedPayload}", secretKey);
                if (computedSignature != signature)
                {
                    return (null, false);
                }

                // 解密 payload
                var decryptedPayload = CryptoHelper.DecryptAES(encryptedPayload, secretKey, iv);
                var payloadJson = CryptoHelper.Base64Decode(decryptedPayload);
                var payload = JsonConvert.DeserializeObject<Payload>(payloadJson);

                // 檢查過期時間
                var currentTimestamp = (int)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
                if (payload.Expires <= currentTimestamp)
                {
                    return (null, false);
                }

                return (payload.UserInfo, true);
            }
            catch
            {
                return (null, false);
            }
        }



        public Token GenerateToken(User user)
        {

            //Token格式 header.payload.signature
            var secretKey = _configuration["JWTSetting:SignKey"];

            var exp = Convert.ToInt32(_configuration["JWTSetting:ExpireTime"]);   //過期時間(秒)
            //確保 IV 符合 AES加密的 限制，否則會拋錯 CryptographicException: 'Specified key is not a valid size for this algorithm.'
            var iv = Guid.NewGuid().ToString().Replace("-","").Substring(0,16);
            //payload  + AES 加密
            var payload = new Payload
            {
                UserInfo = user,
                //Unix 時間戳，理解為Token產生的時間
                Expires = Convert.ToInt32(
                    (DateTime.Now.AddSeconds(exp) -
                     new DateTime(1970, 1, 1)).TotalSeconds)
            };

            var json = JsonConvert.SerializeObject(payload);

            var base64 = CryptoHelper.Base64Encode(json);

            var encrypt = CryptoHelper.EncryptAES(base64, secretKey, iv);

            var signature = CryptoHelper.GenerateHmacSha256($"{iv}.{encrypt}",secretKey);

            return new Token
            {
                AccessToken = $"{iv}.{encrypt}.{signature}",
                RefreshToken =Guid.NewGuid().ToString(),
                Expires=exp
            };
        }

    }

    public class Token
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public int Expires { get; set; }
    }

    public class Payload
    {    
        public User UserInfo { get; set; }
        public int Expires { get; set; }
    }

    public class User {

        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
    }
}

