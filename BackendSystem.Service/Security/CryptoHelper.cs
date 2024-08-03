using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;


namespace BackendSystem.Service.Security
{
    public class CryptoHelper
    {
        ///<summary>
        ///字串加密 AES
        /// </summary>
        /// <returns>加密後16位元字串</returns>
        public static string EncryptAES(string data, string hashKey, string hashIV)
        {
            if (string.IsNullOrEmpty(data) || string.IsNullOrEmpty(hashKey) || string.IsNullOrEmpty(hashIV))
            {
                throw new ArgumentNullException("Data, Hash Key, and Hash IV cannot be null or empty.");
            }

            byte[] dataKey = Encoding.UTF8.GetBytes(hashKey);
            byte[] dataIV = Encoding.UTF8.GetBytes(hashIV);

            using (Aes aes = Aes.Create())
            {
                aes.Key = dataKey;
                aes.IV = dataIV;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, aes.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        byte[] dataBytes = Encoding.UTF8.GetBytes(data);
                        cryptoStream.Write(dataBytes, 0, dataBytes.Length);
                        cryptoStream.FlushFinalBlock();
                    }
                    byte[] encryptedBytes = memoryStream.ToArray();
                    return ByteArrayToHexString(encryptedBytes);
                }
            }
        }

        ///<summary>
        ///AES 解密字串
        /// </summary>
        /// <returns>解密後字串</returns>
        public static string DecryptAES(string encryptedHex, string hashKey, string hashIV)
        {
            if (string.IsNullOrEmpty(encryptedHex) || string.IsNullOrEmpty(hashKey) || string.IsNullOrEmpty(hashIV))
            {
                throw new ArgumentNullException("Encrypted hex string, Hash Key, and Hash IV cannot be null or empty.");
            }

            byte[] encryptedBytes = HexStringToByteArray(encryptedHex);
            byte[] dataKey = Encoding.UTF8.GetBytes(hashKey);
            byte[] dataIV = Encoding.UTF8.GetBytes(hashIV);

            using (Aes aes = Aes.Create())
            {
                aes.Key = dataKey;
                aes.IV = dataIV;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                using (MemoryStream memoryStream = new MemoryStream(encryptedBytes))
                {
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, aes.CreateDecryptor(), CryptoStreamMode.Read))
                    {
                        using (MemoryStream decryptedStream = new MemoryStream())
                        {
                            cryptoStream.CopyTo(decryptedStream);
                            byte[] decryptedBytes = decryptedStream.ToArray();
                            return Encoding.UTF8.GetString(decryptedBytes);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 字串加密 SHA256
        /// </summary>
        /// <param name="source">加密前字串</param>
        /// <returns>加密後字串</returns>
        public static string EncryptSHA256(string source)
        {
            string? result = string.Empty;

            using (SHA256 algorithm = SHA256.Create())
            {
                var hash = algorithm.ComputeHash(Encoding.UTF8.GetBytes(source));

                if (hash != null)
                {
                    result = BitConverter.ToString(hash)?.Replace("-", string.Empty)?.ToUpper();
                }

            }

            return result;
        }

        /// <summary>
        /// Byte陣列轉16進制
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns>16進制字串</returns>
        private static string ByteArrayToHexString(byte[] bytes)
        {
            StringBuilder hex = new StringBuilder(bytes.Length * 2);
            foreach (byte b in bytes)
            {
                hex.AppendFormat("{0:x2}", b);
            }
            return hex.ToString();
        }

        /// <summary>
        /// 16進制轉Byte陣列
        /// </summary>
        /// <param name="hex"></param>
        /// <returns>Byte陣列</returns>
        /// <exception cref="ArgumentException"></exception>
        private static byte[] HexStringToByteArray(string hex)
        {
            if (hex.Length % 2 != 0)
            {
                throw new ArgumentException("Hex string length must be even.");
            }

            byte[] bytes = new byte[hex.Length / 2];
            for (int i = 0; i < hex.Length; i += 2)
            {
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            }
            return bytes;
        }

        ///<summary>
        ///HMACSHA256 雜湊
        /// </summary>
        /// <returns></returns>
        public static string GenerateHmacSha256(string key, string message)
        {
            // 將密鑰和消息轉換為Byte
            byte[] keyBytes = Encoding.UTF8.GetBytes(key);
            byte[] messageBytes = Encoding.UTF8.GetBytes(message);

            // 創建 HMACSHA256，並使用key
            using (var hmacSha256 = new HMACSHA256(keyBytes))
            {
                // HMAC
                byte[] hashBytes = hmacSha256.ComputeHash(messageBytes);

                // 將字節數組轉換為十六進制字符串
                return ByteArrayToHexString(hashBytes);
            }
        }

        /// <summary>
        /// base64 編碼
        /// </summary>
        /// <param name="plainText"></param>
        /// <returns></returns>
        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }

        /// <summary>
        /// base64 解碼
        /// </summary>
        /// <param name="base64EncodedData"></param>
        /// <returns></returns>
        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = Convert.FromBase64String(base64EncodedData);
            return Encoding.UTF8.GetString(base64EncodedBytes);
        }


    }
}
