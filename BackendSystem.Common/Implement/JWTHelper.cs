using BackendSystem.Common.Interface;
using BackendSystem.Common.Model;
using BackendSystem.Common.Dtos;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BackendSystem.Common.Enum;
using BackendSystem.Common.ResultModel;

namespace BackendSystem.Common.Implement
{
    public class JWTHelper: IJWTHelper
    {
        private readonly TokenSettings _appSettings;

        public JWTHelper(IOptionsMonitor<TokenSettings> appSettings)
        {
            _appSettings = appSettings.CurrentValue;
        }

        public string GenerateToken(User user,TokenType type)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] {
                        new Claim("userId", user.Id.ToString()),
                        new Claim("email",user.Email),
                        new Claim("role",user.Role),
                        new Claim("type","access")
                    }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var claims = tokenDescriptor.Subject;
            switch (type)
            {
                case TokenType.Login:
                    claims.AddClaim(new Claim("purpose", "login"));
                    break;
                case TokenType.VerifyEmail:
                    claims.AddClaim(new Claim("purpose", "verify_email"));
                    break;
                case TokenType.Refresh:
                    claims.AddClaim(new Claim("purpose", "refresh_token"));
                    break;
            }

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public int? ValidateToken(string token)
        {
            if (token == null)
                return null;

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var userId = int.Parse(jwtToken.Claims.First(x => x.Type == "id").Value);
                return userId;
            }
            catch
            {
                return null;
            }
        }
    }
}
