using BackendSystem.Common.Dtos;

namespace BackendSystem.Common.Interface
{
    public interface IJWTHelper
    {
        public string GenerateToken(User user);
        public int? ValidateToken(string token);

    }
}
