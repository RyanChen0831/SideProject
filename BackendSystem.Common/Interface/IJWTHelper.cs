using BackendSystem.Common.Dtos;
using BackendSystem.Common.Enum;
using BackendSystem.Common.ResultModel;

namespace BackendSystem.Common.Interface
{
    public interface IJWTHelper
    {
        /// <summary>
        /// BCrypt雜湊演算法產token
        /// </summary>
        /// <param name="user"></param>
        /// <param name="type"></param>
        /// <returns>Base64Url的字串</returns>
        public string GenerateToken(User user,TokenType type);
        public ValidateTokenResult? ValidateToken(string token);

    }
}
