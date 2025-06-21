namespace BackendSystem.Respository.CommandModels
{
    internal class UpdateOrderStatusCommandModel
    {
        public int OrderId { get; set; }
        public string OrderStatus { get; set; }
        public string ShippingCompany { get; set; }
        public string TrackingNumber { get; set; }
        public string AdminNote { get; set; }
        public int UpdatedBy { get; set; }
    }
}
