namespace BackendSystem.Respository.ResultModels
{
    public class OrderDataModel
    {
        public int OrderId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public int UnitPrice { get; set; }
        public int SubTotal { get; set; }
        public string? ShippingAddress { get; set; }
        public string? Payment { get; set; }
        public string? PaymentStatus { get; set; }
        public DateTime OrderDate { get; set; }
    }
}
