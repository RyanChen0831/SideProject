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

    public class OrderDetailCondition
    {
        public string? OrderId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public int UnitPrice { get; set; }
        public int SubTotal { get; set; }

    }

}
