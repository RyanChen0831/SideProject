namespace BackendSystem.Common.ResultModel
{
    /// <summary>
    /// 採用靜態工廠的方式來封裝驗證Token後的結果
    /// </summary>
    public class ValidateTokenResult
    {
        public bool IsValid { get; private set; }
        public int UserId { get; private set; }
        public string? Email { get; private set; }
        public string? Role { get; private set; }
        public string? Purpose { get; private set; }
        public string? ErrorMessage { get; private set; }
        public DateTime? ExpirationTime { get; private set; }

        private ValidateTokenResult() { }
        public static ValidateTokenResult Invalid(string? errorMessage = null) => new ValidateTokenResult { IsValid = false, ErrorMessage = errorMessage };
        public static ValidateTokenResult Success(int userId, string email, string role, string purpose, DateTime? expire) =>
            new ValidateTokenResult
            {
                IsValid = true,
                UserId = userId,
                Email = email,
                Role = role,
                Purpose = purpose,
                ExpirationTime = expire
            };

    }
}
