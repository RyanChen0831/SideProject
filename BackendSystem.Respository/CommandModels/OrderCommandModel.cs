namespace BackendSystem.Respository.CommandModels
{
    public class OrderCommandModel
    {
        public string? OrderId { get; set; }
        public int MemberId { get; set; }
        public int TotalAmount { get; set; }
        public string? ShippingAddress { get; set; }
        public string? ShippingStatus { get; set; }
        public string? Payment { get; set; }
        public string? PaymentStatus { get; set; }
    }
}
