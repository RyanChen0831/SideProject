using BackendSystem.Service.Security;

namespace BackendSystem.Service.Interface
{
    public interface ITokenService
    {
        public Token GenerateToken(User user);
        public (User?,bool) DecodeToken(Token token);
    }
}
