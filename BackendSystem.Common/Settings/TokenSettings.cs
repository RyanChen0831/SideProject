namespace BackendSystem.Common.Model
{
    public class TokenSettings
    {
        public string Secret { get; set; }
        public string Issuer { get; set; }
        public int ExpireDays { get; set; }
    }
}
