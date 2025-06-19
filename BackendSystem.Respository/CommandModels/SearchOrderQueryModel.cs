namespace BackendSystem.Respository.CommandModels
{
    public class SearchOrderQueryModel
    {
        public string? OrderId { get; set; }
        public string? MemberId { get; set; }
        public string? MemberName { get; set; }
        public string? ShippingStatus { get; set; }
        public string? ShippingAddress { get; set; }
        public string? OrderStatus { get; set; }
        public string? Payment { get; set; }
        public bool? IsCancel { get; set; }
        public bool? IsDeleted { get; set; }
    }
}
