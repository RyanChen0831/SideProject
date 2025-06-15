

namespace BackendSystem.Service.Dtos
{
    public class OrderDetailDTO
    {
        public string ProductName { get; set; }
        public string ProductImage { get; set; }
        public int Quantity { get; set; }
        public int SubTotal { get; set; }
    }
}
