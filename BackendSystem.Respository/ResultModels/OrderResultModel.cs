namespace BackendSystem.Respository.ResultModels
{
    public class OrderResultModel
    {
        public string OrderId { get; set; }
        public string OrderDate { get; set; }
        public int TotalAmount { get; set; }
        public string ShippingAddress { get; set; }
        public string ShippingStatus { get; set; }
        public string Payment { get; set; }
        public string PaymentStatus { get; set; }
        public List<OrderDetailResultModel> OrderDetail { get; set; }
    }
}
