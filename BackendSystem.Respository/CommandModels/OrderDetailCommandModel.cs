
namespace BackendSystem.Respository.CommandModels
{
    public class OrderDetailCommandModel
    {
        public string? OrderId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public int UnitPrice { get; set; }
        public int SubTotal { get; set; }
    }
}
